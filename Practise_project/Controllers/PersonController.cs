using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; // 追加
using Practise_project.Data; // 追加
using Practise_project.Models;
using System.Diagnostics.Eventing.Reader;

namespace Practise_project.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;

        // コンストラクタでDI（依存性注入）によりDbContextを受け取る
        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. 編集画面を表示する処理 (GET)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // データベースから対象のPersonデータを取得
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            // Entityから画面用のViewModelへデータを詰め替える
            var model = new PersonSearchViewModel
            {
                SearchGivenName = person.GivenName,
                SearchSurName = person.SurName,
                SearchLocalLanguageName = person.LocalLanguageName,
                SearchPersonType = person.PersonType,
                SearchEmail = person.Email,

                // ここにAgeを追加！データベースの値を画面用のプロパティに入れます
                SearchAge = person.Age,

                // ドロップダウンの選択肢を取得
                PersonTypeOptions = GetPersonTypeOptions()
            };

            ViewData["PersonId"] = person.Id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PersonSearchViewModel model)
        {
            if (!model.SearchAge.HasValue || !ModelState.IsValid)
            {
                // 画面に「年齢を入れてください」とエラーを明示する
                if (!model.SearchAge.HasValue)
                {
                    ModelState.AddModelError("SearchAge", "年齢は必須入力です。");
                }

                // 選択肢を再設定して編集画面に戻す（これで絶対にフリーズしません）
                model.PersonTypeOptions = GetPersonTypeOptions();
                ViewData["PersonId"] = id;
                return View(model);
            }
            // データベースから現在のデータを取得
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            // 画面（ViewModel）で入力された値を、データベース用（Entity）に上書きする
            person.GivenName = model.SearchGivenName!;
            person.SurName = model.SearchSurName!;
            person.LocalLanguageName = model.SearchLocalLanguageName;
            person.PersonType = model.SearchPersonType!;
            person.Email = model.SearchEmail;

            //ここにAgeを追加！画面で入力された数値をDBのモデルにセットします
            person.Age = model.SearchAge.Value;

            // データベースを更新して保存
            _context.Update(person);
            await _context.SaveChangesAsync();

            // 保存が終わったら検索画面（一覧）に戻る
            return RedirectToAction(nameof(Search));
        }

        private List<SelectListItem> GetPersonTypeOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Employee", Text = "Employee" },
                new SelectListItem { Value = "Customer", Text = "Customer" }
            };
        }

        [HttpGet]
        public IActionResult Search()
        {
            var model = new PersonSearchViewModel
            {
                PersonTypeOptions = GetPersonTypeOptions()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExecuteSearch(PersonSearchViewModel model)
        {
            model.PersonTypeOptions = GetPersonTypeOptions();

            if (!ModelState.IsValid)
            {
                return View("Search", model);
            }

            // 1. クエリのベースを作成（IQueryable）
            var query = _context.Persons.AsQueryable();

            // 2. 検索条件があったらWHERE句を追加
            if (!string.IsNullOrEmpty(model.SearchGivenName))
            {
                query = query.Where(p => p.GivenName == model.SearchGivenName);
            }

            if (!string.IsNullOrEmpty(model.SearchSurName))
            {
                query = query.Where(p => p.SurName == model.SearchSurName);
            }

            if (!string.IsNullOrEmpty(model.SearchLocalLanguageName))
            {
                query = query.Where(p => p.LocalLanguageName == model.SearchLocalLanguageName);
            }

            if (!string.IsNullOrEmpty(model.SearchPersonType))
            {
                query = query.Where(p => p.PersonType == model.SearchPersonType);
            }

            if (!string.IsNullOrEmpty(model.SearchEmail))
            {
                query = query.Where(p => p.Email == model.SearchEmail);
            }

            // 3. データベースからデータを非同期で取得し、ViewModelの型に変換（マッピング）する
            model.SearchResults = await query
                .Select(p => new PersonSearchResultItem
                {
                    Id = p.Id,
                    GivenName = p.GivenName,
                    SurName = p.SurName,
                    PersonType = p.PersonType,
                    LocalLanguageName = p.LocalLanguageName,
                    Email = p.Email
                }).ToListAsync();

            model.IsSearched = true;

            return View("Search", model);
        }
    }
}