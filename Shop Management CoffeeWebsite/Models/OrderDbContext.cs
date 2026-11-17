using System.Data.Entity;

namespace CafeManagement.Models
{
    // Dùng chung database MimiCoffeeDB. Bảng Orders, OrderItems được tự tạo bằng SQL
    // nếu chưa có -> không cần Migration.
    public class OrderDbContext : DbContext
    {
        private static readonly object _lock = new object();
        private static bool _ready;

        static OrderDbContext()
        {
            Database.SetInitializer<OrderDbContext>(null);
        }

        public OrderDbContext() : base("name=MimiCoffeeDB")
        {
            EnsureTables();
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        private void EnsureTables()
        {
            if (_ready) return;
            lock (_lock)
            {
                if (_ready) return;

                using (var coffee = new CoffeeDbContext())
                {
                    coffee.Database.CreateIfNotExists();
                }

                Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, @"
IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
CREATE TABLE dbo.Orders (
    Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId        INT            NULL,
    CustomerName  NVARCHAR(100)  NULL,
    Status        NVARCHAR(20)   NOT NULL,
    PaymentMethod NVARCHAR(30)   NULL,
    CreatedAt     DATETIME       NOT NULL,
    PaidAt        DATETIME       NULL,
    PaidAmount    DECIMAL(18,2)  NULL
);

IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
CREATE TABLE dbo.OrderItems (
    Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    OrderId     INT            NOT NULL,
    ProductId   INT            NOT NULL,
    ProductName NVARCHAR(150)  NOT NULL,
    UnitPrice   DECIMAL(18,2)  NOT NULL,
    Quantity    INT            NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId)
        REFERENCES dbo.Orders (Id) ON DELETE CASCADE
);");
                _ready = true;
            }
        }
    }
}
