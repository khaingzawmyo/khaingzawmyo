using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Practise_project.Models
{
    public class PersonSearchViewModel
    {
        // --- 検索条件 ---
        [Required(ErrorMessage = "Given Nameは必須入力です。")]
        public string? SearchGivenName { get; set; }

        [Required(ErrorMessage = "SurNameは必須入力です。")]
        public string? SearchSurName { get; set; }

        [Required(ErrorMessage = "SearchLocalLanguageNameは必須入力です。")]
        public string? SearchLocalLanguageName { get; set; }

        [Required(ErrorMessage = "Person Typeを選択してください。")]
        public string? SearchPersonType { get; set; }


        [Required(ErrorMessage = "メールアドレスを入力してください。")]
        [EmailAddress(ErrorMessage = "正しいメールアドレスの形式ではありません。")]
        // 末尾が .com で終わるかチェックする正規表現
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$", ErrorMessage = "メールアドレスは '.com' で終わる必要があります。")]
        public string? SearchEmail { get; set; }

        public int? SearchAge { get; set; }

        public bool IsSearched { get; set; } = false;


        // --- ドロップダウンの選択肢 ---
        public List<SelectListItem> PersonTypeOptions { get; set; } = new();


        // --- 検索結果 ---
        public List<PersonSearchResultItem>? SearchResults { get; set; }
    }

    // 検索結果の1行を表すクラス
    public class PersonSearchResultItem
    {
        public int Id { get; set; }
        public string? GivenName { get; set; }
        public string? SurName { get; set; }
        public string? PersonType { get; set; }
        public string? LocalLanguageName { get; set; }
        public string? Email { get; set; }

        public int? Age { get; set; }

    }
}