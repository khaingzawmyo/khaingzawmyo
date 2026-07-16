//using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; // 追加
using Practise_project.Data; // 追加
using Practise_project.Models;
//using System.Diagnostics.Eventing.Reader;

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
                    CreatePersonName = i.CreatePerson != null ? i.CreatePerson.GivenName : "Unknown",
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
                Text = p.GivenName
            }).ToList();
        }
    }
}
