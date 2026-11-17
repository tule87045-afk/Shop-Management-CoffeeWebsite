using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(150)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        // Dạng: iterations.salt.hash (PBKDF2) - không lưu mật khẩu thô
        [Required, StringLength(200)]
        public string PasswordHash { get; set; }

        [Required, StringLength(20)]
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
