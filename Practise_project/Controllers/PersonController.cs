using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; // 追加
using Practise_project.Data; // 追加
using Practise_project.Models;

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

        private List<SelectListItem> GetPersonTypeOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Employee", Text = "Employee" },
                new SelectListItem { Value = "Manager", Text = "Manager" },
                new SelectListItem { Value = "Client", Text = "Client" }
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
                query = query.Where(p => p.GivenName.Contains(model.SearchGivenName));
            }

            if (!string.IsNullOrEmpty(model.SearchSurName))
            {
                query = query.Where(p => p.SurName.Contains(model.SearchSurName));
            }

            if (!string.IsNullOrEmpty(model.SearchPersonType))
            {
                query = query.Where(p => p.PersonType == model.SearchPersonType);
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

            return View("Search", model);
        }
    }
}