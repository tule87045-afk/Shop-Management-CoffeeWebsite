using System;
using System.Data.Entity;
using System.Linq;

namespace CafeManagement.Models
{
    // Dùng chung database MimiCoffeeDB. Các bảng Cats, CatSubmissions, AdoptionRequests
    // được tự tạo bằng SQL nếu chưa có -> không cần Migration.
    public class AdoptionDbContext : DbContext
    {
        private static readonly object _lock = new object();
        private static bool _ready;

        static AdoptionDbContext()
        {
            Database.SetInitializer<AdoptionDbContext>(null);
        }

        public AdoptionDbContext() : base("name=MimiCoffeeDB")
        {
            EnsureTables();
        }

        public DbSet<Cat> Cats { get; set; }
        public DbSet<CatSubmission> CatSubmissions { get; set; }
        public DbSet<AdoptionRequest> AdoptionRequests { get; set; }

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
IF OBJECT_ID(N'dbo.Cats', N'U') IS NULL
CREATE TABLE dbo.Cats (
    Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name         NVARCHAR(100)  NOT NULL,
    AgeText      NVARCHAR(50)   NULL,
    Gender       NVARCHAR(10)   NULL,
    Breed        NVARCHAR(100)  NULL,
    Color        NVARCHAR(50)   NULL,
    Description  NVARCHAR(1000) NULL,
    ImageUrl     NVARCHAR(300)  NULL,
    IsVaccinated BIT            NOT NULL,
    Status       NVARCHAR(20)   NOT NULL,
    CreatedAt    DATETIME       NOT NULL
);

IF OBJECT_ID(N'dbo.CatSubmissions', N'U') IS NULL
CREATE TABLE dbo.CatSubmissions (
    Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    OwnerName    NVARCHAR(100)  NOT NULL,
    Phone        NVARCHAR(20)   NOT NULL,
    Email        NVARCHAR(150)  NULL,
    CatName      NVARCHAR(100)  NOT NULL,
    AgeText      NVARCHAR(50)   NULL,
    Gender       NVARCHAR(10)   NULL,
    Breed        NVARCHAR(100)  NULL,
    IsVaccinated BIT            NOT NULL,
    Description  NVARCHAR(1000) NULL,
    Reason       NVARCHAR(500)  NULL,
    ImageUrl     NVARCHAR(300)  NULL,
    Status       NVARCHAR(20)   NOT NULL,
    CreatedAt    DATETIME       NOT NULL,
    UserId       INT            NULL
);

IF OBJECT_ID(N'dbo.AdoptionRequests', N'U') IS NULL
CREATE TABLE dbo.AdoptionRequests (
    Id        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CatId     INT            NOT NULL,
    FullName  NVARCHAR(100)  NOT NULL,
    Phone     NVARCHAR(20)   NOT NULL,
    Email     NVARCHAR(150)  NULL,
    Address   NVARCHAR(250)  NULL,
    Note      NVARCHAR(500)  NULL,
    Status    NVARCHAR(20)   NOT NULL,
    CreatedAt DATETIME       NOT NULL,
    UserId    INT            NULL
);");

                // Thêm vài bé mèo mẫu khi bảng còn trống
                if (!Cats.Any())
                {
                    Cats.Add(NewCat("Mướp", "3 tháng", "Đực", "Mèo ta", "Vàng vằn", "Nghịch ngợm, thích đuổi bắt bóng len và rất ham ăn. Đã quen với người lạ.", true));
                    Cats.Add(NewCat("Bơ", "5 tháng", "Cái", "Anh lông ngắn", "Xám xanh", "Hiền lành, thích nằm lên đùi và kêu gừ gừ cả ngày.", true));
                    Cats.Add(NewCat("Mochi", "2 tháng", "Cái", "Mèo ta", "Trắng", "Bé nhỏ nhất đàn, nhút nhát lúc đầu nhưng rất bám người khi quen.", false));
                    Cats.Add(NewCat("Tiểu Hổ", "1 tuổi", "Đực", "Mèo ta", "Vằn hổ", "Năng động, thích leo trèo. Hợp với gia đình có sân hoặc ban công an toàn.", true));
                    Cats.Add(NewCat("Cam", "4 tháng", "Đực", "Mèo vàng", "Cam", "Thân thiện, thích chơi với trẻ nhỏ và các bạn mèo khác.", true));
                    Cats.Add(NewCat("Sữa", "8 tháng", "Cái", "Ba tư lai", "Trắng kem", "Điềm đạm, thích được chải lông. Cần chăm sóc bộ lông thường xuyên.", true));
                    SaveChanges();
                }

                _ready = true;
            }
        }

        private static Cat NewCat(string name, string age, string gender, string breed, string color, string desc, bool vaccinated)
        {
            return new Cat
            {
                Name = name,
                AgeText = age,
                Gender = gender,
                Breed = breed,
                Color = color,
                Description = desc,
                IsVaccinated = vaccinated,
                Status = "Available",
                CreatedAt = DateTime.Now
            };
        }
    }
}
