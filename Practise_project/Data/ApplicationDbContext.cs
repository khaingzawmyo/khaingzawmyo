using Microsoft.EntityFrameworkCore;

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
    }

    public class PersonEntitiy
    {
        public int Id { get; set; }
        public string GivenName { get; set; } = string.Empty;
        public string SurName { get; set; } = string.Empty;
        public string? LocalLanguageName { get; set; } = string.Empty;
        public string PersonType { get; set; } = string.Empty;
        public string? Email { get; set; }

        public int? Age { get; set; }

    }
}
