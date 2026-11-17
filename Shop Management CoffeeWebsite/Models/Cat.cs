using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models
{
    // Mèo đang chờ được nhận nuôi tại Mimi Cat Cafe
    [Table("Cats")]
    public class Cat
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string AgeText { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(100)]
        public string Breed { get; set; }

        [StringLength(50)]
        public string Color { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(300)]
        public string ImageUrl { get; set; }

        public bool IsVaccinated { get; set; }

        // Available | Adopted
        [Required, StringLength(20)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    // Người dùng gửi mèo cho shop
    [Table("CatSubmissions")]
    public class CatSubmission
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string OwnerName { get; set; }

        [Required, StringLength(20)]
        public string Phone { get; set; }

        [StringLength(150)]
        public string Email { get; set; }

        [Required, StringLength(100)]
        public string CatName { get; set; }

        [StringLength(50)]
        public string AgeText { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(100)]
        public string Breed { get; set; }

        public bool IsVaccinated { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }

        [StringLength(300)]
        public string ImageUrl { get; set; }

        // Pending | Approved | Rejected
        [Required, StringLength(20)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? UserId { get; set; }
    }

    // Người dùng đăng ký nhận nuôi một bé mèo
    [Table("AdoptionRequests")]
    public class AdoptionRequest
    {
        public int Id { get; set; }

        public int CatId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(20)]
        public string Phone { get; set; }

        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(500)]
        public string Note { get; set; }

        // Pending | Confirmed | Rejected
        [Required, StringLength(20)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? UserId { get; set; }
    }
}
