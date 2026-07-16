using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practise_project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //これが実際のDB「Person」テーブルに対応します。
        public DbSet<PersonEntitiy> Persons { get; set; }

        //これが実際のDB「invoice」テーブルに対応します。
        public DbSet<InvoiceEntity> Invoice { get; set; }

        //これが実際のDB「invoice」テーブルに対応します。
        public DbSet<InvoiceItemEntitiy> Invoice_item { get; set; }
    }

    public class PersonEntitiy
    {
        public int Id { get; set; }
        public string? GivenName { get; set; }
        public string? SurName { get; set; }
        public string? LocalLanguageName { get; set; } 
        public string? PersonType { get; set; }
        public string? Email { get; set; }

        public int Age { get; set; }

    }

    public class InvoiceEntity
    {
        [Key]
        public int Invoice_id { get; set; }
        public string? Invoice_no { get; set; }
        public int Create_person_id { get; set; }
        public decimal Total_amount { get; set; }
        public string? Customer_name { get; set; }
        public string? Remarks { get; set; }
        public bool Void_flag { get; set; }
        public DateTime Entry_date { get; set; }
        public DateTime? Update_date { get; set; }
        [ForeignKey("Create_person_id")]
        public virtual PersonEntitiy? CreatePerson { get; set; }
    }

    public class InvoiceItemEntitiy
    {
        [Key]
        public int Invoice_item_id { get; set; }
        public int Invoice_id { get; set; }
        public string? Charge_description { get; set; }
        public int Create_person_id { get; set; }
        public decimal? Revenue_amount { get; set; }
        public decimal? Cost_amount { get; set; }
        public DateTime Entry_date { get; set; }
        public DateTime? Update_date { get; set; }
        public short Rowver { get; set; }
    }
}
