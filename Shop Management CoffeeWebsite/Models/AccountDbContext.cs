using System.Data.Entity;

namespace CafeManagement.Models
{
    // Dùng chung database MimiCoffeeDB với CoffeeDbContext.
    // Bảng Users được tự tạo bằng SQL nếu chưa có -> không cần Migration, không cần chạy script tay.
    public class AccountDbContext : DbContext
    {
        private static readonly object _lock = new object();
        private static bool _ready;

        static AccountDbContext()
        {
            Database.SetInitializer<AccountDbContext>(null);
        }

        public AccountDbContext() : base("name=MimiCoffeeDB")
        {
            EnsureTable();
        }

        public DbSet<User> Users { get; set; }

        private void EnsureTable()
        {
            if (_ready) return;
            lock (_lock)
            {
                if (_ready) return;

                // Đảm bảo database tồn tại (do CoffeeDbContext tạo nếu chưa có)
                using (var coffee = new CoffeeDbContext())
                {
                    coffee.Database.CreateIfNotExists();
                }

                Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, @"
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        FullName     NVARCHAR(100) NOT NULL,
        Email        NVARCHAR(150) NOT NULL,
        Phone        NVARCHAR(20)  NULL,
        PasswordHash NVARCHAR(200) NOT NULL,
        Role         NVARCHAR(20)  NOT NULL,
        CreatedAt    DATETIME      NOT NULL
    );
    CREATE UNIQUE INDEX UX_Users_Email ON dbo.Users (Email);
END");
                _ready = true;
            }
        }
    }
}
