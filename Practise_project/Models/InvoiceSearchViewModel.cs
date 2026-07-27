using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Practise_project.Models
{
    public class InvoiceSearchViewModel
    {
        // --- 検索条件 ---

        // ① Invoice No（ワイヤーフレームで必須「*」マークがあるためRequiredを有効にしています）
        //[Required(ErrorMessage = "Invoice Noは必須入力です。")]
        [StringLength(50, ErrorMessage = "50文字以内で入力してください。")]
        public String? SearchInvoiceNo { get; set; }
       // public int SearchInvoiceItemNo { get; set; }


        // ② Create Person (ドロップダウンで選択されたPersonのIdを受け取る)
        public int? SearchCreatePersonId { get; set; }

        // ③ To Create Date (From側：期間の開始日)
        [DataType(DataType.Date)]
        public DateTime? SearchCreateDateFrom { get; set; }

        // ④ To Create Date (To側：期間の終了日)
        [DataType(DataType.Date)]
        public DateTime? SearchCreateDateTo { get; set; }

        // 検索ボタンが押されたかどうかの判定フラグ
        public bool IsSearched { get; set; } = false;

        public string? Customer_name { get; set; }
        public decimal Total_amount { get; set; }
        public string? Remarks { get; set; }
        public string? Charge_description { get; set; }
        public decimal? Revenue_amount { get; set; }
        public decimal? Cost_amount { get; set; }

        // --- ドロップダウンの選択肢 ---
        // ② Create Person のセレクトボックス用リスト
        public List<SelectListItem> CreatePersonOptions { get; set; } = new();

        // --- 検索結果 ---
        public List<InvoiceSearchResultItem>? SearchResults { get; set; }

        public List<InvoiceItemInputModel> Details { get; set; } = new List<InvoiceItemInputModel>();
    }

    // 検索結果の1行を表すクラス
    public class InvoiceSearchResultItem
    {
        public int InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }

        //ここに紐付いた作成者の名前が入ります
        public string? CreatePersonName { get; set; }

        // 画像の「Employee」などの区分を入れるプロパティ
        public string? CreatePersonType { get; set; }

        public DateTime EntryDate { get; set; }
        public string? Remarks { get; set; }  
    }
    public class InvoiceItemInputModel
    {
        public string? ChargeDesc { get; set; }
        public decimal RevenueAmount { get; set; }
        public decimal CostAmount { get; set; }
    }
}