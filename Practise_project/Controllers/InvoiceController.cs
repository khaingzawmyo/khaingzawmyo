using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; // 追加
using Microsoft.IdentityModel.Tokens;
using Practise_project.Data; // 追加
using Practise_project.Models;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata.Ecma335;

namespace Practise_project.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        // コンストラクタでDbContextをインジェクション
        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult>Search()
        {
            var viewModel = new InvoiceSearchViewModel();

            // ドロップダウン用の作成者リストを設定
            await PopulateCreatePersonOptionsAsync(viewModel);

            // 初回表示時は検索前なので、検証エラーメッセージを表示させないようにクリア
            ModelState.Clear();
            viewModel.IsSearched = false;

            return View(viewModel);
        }

        // ==========================================
        // 2. 検索実行時 (POST または GET)
        // ※バリデーション（Required）があるため、通常はPOSTか、
        // もしくは引数でViewModelを受け取る形にします。ここではPOSTでの例とします。
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> Search(InvoiceSearchViewModel viewModel)
        {
            // 再度ドロップダウンの選択肢を設定（画面再表示の際に必要）
            await PopulateCreatePersonOptionsAsync(viewModel);

            // 入力チェック（Invoice Noが空などの場合）に引っかかったら、そのまま画面を返す
            if (!ModelState.IsValid)
            {
                viewModel.IsSearched = false;
                return View(viewModel);
            }

            viewModel.IsSearched = true;

            // DBからInvoiceデータを取得するクエリ（PersonテーブルもIncludeで結合）
            var result = _context.Invoice
                .Include(i => i.CreatePerson) // Model.csで設定したナビゲーションプロパティ
                .AsQueryable();

            // --- 検索条件でのフィルタリング ---

            // ① Invoice No で絞り込み（前方一致や部分一致など。ここでは部分一致）
            if (!string.IsNullOrEmpty(viewModel.SearchInvoiceNo))
            {
                result = result.Where(i => i.Invoice_no != null && i.Invoice_no.Contains(viewModel.SearchInvoiceNo));
            }

            if (viewModel.SearchCreatePersonId.HasValue)
            {
                result = result.Where(i => i.Create_person_id == viewModel.SearchCreatePersonId.Value);
            }

            // ③ 登録日 From で絞り込み
            if (viewModel.SearchCreateDateFrom.HasValue)
            {
                result = result.Where(i => i.Entry_date >= viewModel.SearchCreateDateFrom.Value);
            }

            // ④ 登録日 To で絞り込み
            if (viewModel.SearchCreateDateTo.HasValue)
            {
                // 日付の境界線（その日の23:59:59までなど）をカバーするため、+1日未満とする
                var toDate = viewModel.SearchCreateDateTo.Value.AddDays(1);
                result = result.Where(i => i.Entry_date < toDate);
            }

            // --- 検索結果をViewModelの型に入れ替え ---
            viewModel.SearchResults = await result
                .Select(i => new InvoiceSearchResultItem
                {
                    InvoiceId = i.Invoice_id,
                    InvoiceNo = i.Invoice_no,
                    //Personテーブルと結合しているので、作成者の名前やタイプが取れる
                    CreatePersonName = i.CreatePerson != null ? i.CreatePerson.GivenName+" "+i.CreatePerson.SurName : "Unknown",
                    CreatePersonType = i.CreatePerson != null ? i.CreatePerson.PersonType : "Employee", // 画像の再現用
                    EntryDate = i.Entry_date,
                    Remarks = i.Remarks
                })
                .ToListAsync();

            return View(viewModel);
        }

        // --- 共通処理：ドロップダウンリストの作成 ---
        private async Task PopulateCreatePersonOptionsAsync(InvoiceSearchViewModel viewModel)
        {
            var persons = await _context.Persons.ToListAsync();
            viewModel.CreatePersonOptions = persons.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                // 表示名はファーストネームなどを指定（要件に合わせて変更してください）
                Text = p.GivenName+" "+p.SurName
            }).ToList();
        }// 編集画面を表示する処理 (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Include で明細 (Invoice_item) も一緒にまとめて取得！
            var invoice = await _context.Invoice
        .Include(i => i.InvoiceItems)
        .FirstOrDefaultAsync(i => i.Invoice_id == id);
            // データベースから対象のPersonデータを取得
            if (invoice == null)
            {
                return NotFound();
            }
     
            // Entiyから画面用のViewModelへデータを詰め替える
            var model = new InvoiceSearchViewModel
            {
                SearchInvoiceNo = invoice.Invoice_no,
                SearchCreatePersonId = invoice.Create_person_id,
                Customer_name = invoice.Customer_name,
                Remarks = invoice.Remarks,
                Total_amount = invoice.Total_amount,
                // ここにAgeを追加！データベースの値を画面用のプロパティに入れます
                Details = invoice.InvoiceItems.Select(item => new InvoiceItemInputModel
                {
                    ChargeDesc = item.Charge_description,
                    RevenueAmount = item.Revenue_amount ?? 0, // decimal? のため null 対策
                    CostAmount = item.Cost_amount ?? 0
                }).ToList()
            };

            await PopulateCreatePersonOptionsAsync(model);
            ViewData["InvoiceItemId"] = invoice.Invoice_id; //Invoice_idを取得しています。
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvoiceSearchViewModel model, string action)
        {
            // -----------------------------------------------------------------
            // 1. 【Voidボタン（削除）】が押された場合の処理
            //// -----------------------------------------------------------------
            if (action == "void")
            {
                // 子（明細）データがあれば取得して削除
                var invoiceItem = await _context.Invoice_item
                    .FirstOrDefaultAsync(item => item.Invoice_id == id);
                if (invoiceItem != null)
                {
                    _context.Invoice_item.Remove(invoiceItem);
                }

                // 親データを取得して削除
                var invoice = await _context.Invoice.FindAsync(id);
                if (invoice != null)
                {
                    _context.Invoice.Remove(invoice);
                }

                // 削除を確定
                await _context.SaveChangesAsync();

                // 検索一覧画面に戻る
                return RedirectToAction(nameof(Search));

            }

            // -----------------------------------------------------------------
            // 2. 【Saveボタン（保存）】が押された場合の通常の処理
            // -----------------------------------------------------------------

            // 入力チェック
            ViewData["InvoiceItemId"] = id;
            if (string.IsNullOrEmpty(model.SearchInvoiceNo))
            {
                ModelState.AddModelError("SearchInvoiceNo", "SearchInvoiceNoは必須入力です。");
            }
            if (model.SearchCreatePersonId == null)
            {
                ModelState.AddModelError("SearchCreatePersonId", "CreatePersonは必須入力です。");
            }
            if (string.IsNullOrEmpty(model.Customer_name))
            {
                ModelState.AddModelError("Customer_name", "Customer_nameは必須入力です。");
            }
            //if (model.Total_amount == 0)
            //{
            //    ModelState.AddModelError("Total_amount", "Total_amountは必須入力です。");
            //}
            if (string.IsNullOrEmpty(model.Remarks))
            {
                ModelState.AddModelError("Remarks", "Remarksは必須入力です。");
            }

            if (!ModelState.IsValid)
            {
                // ドロップダウンの選択肢を再構築（これを行わないと画面のドロップダウンが空になります）
                await PopulateCreatePersonOptionsAsync(model);

                // エラーメッセージ付きで元の入力画面を再表示
                return View(model);
            }

            // データベースから現在の親データを取得
            var currentInvoice = await _context.Invoice.FindAsync(id);
            if (currentInvoice == null)
            {
                return NotFound();
            }

            // データベースから現在の子（明細）データを取得
            var currentInvoiceItem = await _context.Invoice_item
                .FirstOrDefaultAsync(item => item.Invoice_id == id);

            bool isNewItem = false;
            if (currentInvoiceItem == null)
            {
                // データベースが自動採番してくれないので、C#側で現在の最大IDを取得して +1 する
                int maxId = await _context.Invoice_item.AnyAsync()
                    ? await _context.Invoice_item.MaxAsync(item => item.Invoice_item_id)
                    : 0;
                currentInvoiceItem = new InvoiceItemEntitiy
                {
                    Invoice_item_id = maxId + 1,
                    Invoice_id = id,
                    Entry_date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    Update_date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    Rowver = 1
                };
                isNewItem = true;
            }

            // 親テーブル（dbo.invoice）への値の上書き
            currentInvoice.Invoice_no = model.SearchInvoiceNo;
            currentInvoice.Create_person_id = model.SearchCreatePersonId!.Value;
            currentInvoice.Customer_name = model.Customer_name;
            currentInvoice.Total_amount = model.Total_amount;
            currentInvoice.Remarks = model.Remarks;

            if (currentInvoice.Entry_date == default)
            {
                currentInvoice.Entry_date = DateTime.Now;
            }
            currentInvoice.Update_date = DateTime.Now; 
           

            // 子テーブル（dbo.invoice_item）への値の上書き
            if (model.Details != null && model.Details.Count > 0)
            {
                currentInvoiceItem.Charge_description = model.Details[0].ChargeDesc ?? "";
                currentInvoiceItem.Revenue_amount = model.Details[0].RevenueAmount;
                currentInvoiceItem.Cost_amount = model.Details[0].CostAmount;
                currentInvoiceItem.Rowver = (short)(currentInvoiceItem.Rowver + 1); // 保存するたびに+1
            }
            else
            {
                currentInvoiceItem.Charge_description = "";
                currentInvoiceItem.Revenue_amount = 0;
                currentInvoiceItem.Cost_amount = 0;
            }

            if (!isNewItem)
            {
                currentInvoiceItem.Update_date = DateTime.Now; // 二度目移行の保存日時（UTC）
                currentInvoiceItem.Rowver = (short)(currentInvoiceItem.Rowver + 1); // 保存するたびに+1
            }

            // データベースへの保存を実行
            try
            {
                _context.Update(currentInvoice);

                if (isNewItem)
                {
                    _context.Invoice_item.Add(currentInvoiceItem);
                }
                else
                {
                    _context.Update(currentInvoiceItem);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Invoice.Any(e => e.Invoice_id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Search));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new InvoiceSearchViewModel(); 
                // ドロップダウンの選択肢を再構築（これを行わないと画面のドロップダウンが空になります）
            await PopulateCreatePersonOptionsAsync(model);
            
            // 「Edit.cshtml」を呼び出して表示する
            return View("Edit", model);
        }
        // 2. 新規作成データをデータベースに保存する（Post）
        //[HttpPost]
        //public async Task<IActionResult> Create(InvoiceSearchViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // 画面のViewModelからデータベースのEntityクラス（例: Person）に詰め替えて保存
        //        var newInvoice = new InvoiceEntity
        //        {
        //            Invoice_no = model.SearchInvoiceNo!,
        //            Create_person_id = model.SearchCreatePersonId!.Value,
        //            Customer_name = model.Customer_name!,
        //            Remarks = model.Remarks!,
        //            //Total_amount = model.Total_amount!,
        //        };
        //        if (newInvoice.Entry_date == default)
        //        {
        //            newInvoice.Entry_date = DateTime.Now;
        //        }
        //        newInvoice.Update_date = DateTime.Now;

        //        _context.Invoice.Add(newInvoice);
        //        await _context.SaveChangesAsync();

        //        // 保存が終わったら検索一覧画面に戻る
        //        return RedirectToAction(nameof(Search));
        //    }

        //    // 入力エラーがある場合は、選択肢を再セットして元の画面に戻す
        //    await PopulateCreatePersonOptionsAsync(model);
        //    ViewData["InvoiceItemId"] = 0;
        //    return View("Edit", model);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceSearchViewModel model)
        {
            // 1. バリデーションチェック
            if (string.IsNullOrEmpty(model.SearchInvoiceNo))
            {
                ModelState.AddModelError("SearchInvoiceNo", "SearchInvoiceNoは必須入力です。");
            }
            if (model.SearchCreatePersonId == null)
            {
                ModelState.AddModelError("SearchCreatePersonId", "CreatePersonは必須入力です。");
            }
            if (string.IsNullOrEmpty(model.Customer_name))
            {
                ModelState.AddModelError("Customer_name", "Customer_nameは必須入力です。");
            }
            var detail = model.Details?.FirstOrDefault();

            if (detail == null || string.IsNullOrEmpty(detail.ChargeDesc))
                ModelState.AddModelError("Details[0].ChargeDesc", "ChargeDescは必須です。");

            if (detail == null || detail.RevenueAmount == 0)
                ModelState.AddModelError("Details[0].RevenueAmount", "RevenueAmountは必須です。");

            if (detail == null || detail.CostAmount == 0)
                ModelState.AddModelError("Details[0].CostAmount", "CostAmountは必須です。");
            //if (model.Total_amount == 0)
            //{
            //    ModelState.AddModelError("Total_amount", "Total_amountは必須入力です。");
            //}
            if (string.IsNullOrEmpty(model.Remarks))
            {
                ModelState.AddModelError("Remarks", "Remarksは必須入力です。");
            }

            if (!ModelState.IsValid)
            {
                // エラー時はドロップダウンの選択肢を再構築して画面を再表示
                await PopulateCreatePersonOptionsAsync(model);
                return View("Edit", model);
            }

            // 2. 親テーブル（Invoice）の新規ID採番（※DB側で自動採番(IDENTITY)でない場合）
            int maxInvoiceId = await _context.Invoice.AnyAsync()
                ? await _context.Invoice.MaxAsync(i => i.Invoice_id)
                : 0;
            int newInvoiceId = maxInvoiceId + 1;

            // 3. 親エンティティの生成
            var newInvoice = new InvoiceEntity // ※型名は実際のEntityクラス名に合わせて調整してください
            {
                Invoice_id = newInvoiceId,
                Invoice_no = model.SearchInvoiceNo,
                Create_person_id = model.SearchCreatePersonId!.Value,
                Customer_name = model.Customer_name,
                Total_amount = model.Total_amount,
                Remarks = model.Remarks,
                Entry_date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                // Update_date は新規の場合、DateTime.Now を入れるか NULL（nullableの場合）にするか設計に合わせて変更してください
                Update_date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                Rowver = 1
            };

            // 4. 子テーブル（InvoiceItem）の新規ID採番
            int maxItemId = await _context.Invoice_item.AnyAsync()
                ? await _context.Invoice_item.MaxAsync(item => item.Invoice_item_id)
                : 0;

            // 5. 子エンティティの生成
            var newInvoiceItem = new InvoiceItemEntitiy
            {
                Invoice_item_id = maxItemId + 1,
                Invoice_id = newInvoiceId,
                Entry_date = DateTime.Now,
                Update_date = DateTime.Now,
                Rowver = 1
            };

            if (model.Details != null && model.Details.Count > 0)
            {
                newInvoiceItem.Charge_description = model.Details[0].ChargeDesc ?? "";
                newInvoiceItem.Revenue_amount = model.Details[0].RevenueAmount;
                newInvoiceItem.Cost_amount = model.Details[0].CostAmount;
            }
            else
            {
                newInvoiceItem.Charge_description = "";
                newInvoiceItem.Revenue_amount = 0;
                newInvoiceItem.Cost_amount = 0;
            }

            // 6. データベースへ追加・保存
            try
            {
                _context.Invoice.Add(newInvoice);
                _context.Invoice_item.Add(newInvoiceItem);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // 必要に応じてログ出力やエラーハンドリング
                ModelState.AddModelError("", "保存中にエラーが発生しました。");
                await PopulateCreatePersonOptionsAsync(model);
                return View("Edit", model);
            }

            // 保存完了後は検索画面へリダイレクト
            return RedirectToAction(nameof(Search));
        }
    }
}
