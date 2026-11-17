using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Models
{
    public class SubmitCatViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên của bạn!")]
        [StringLength(100)]
        [Display(Name = "Họ và tên người gửi")]
        public string OwnerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        [RegularExpression(@"^(0|\+84)\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ!")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên bé mèo!")]
        [StringLength(100)]
        [Display(Name = "Tên bé mèo")]
        public string CatName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tuổi của bé!")]
        [StringLength(50)]
        [Display(Name = "Tuổi")]
        public string AgeText { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính!")]
        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [StringLength(100)]
        [Display(Name = "Giống mèo")]
        public string Breed { get; set; }

        [Display(Name = "Đã tiêm phòng")]
        public bool IsVaccinated { get; set; }

        [Required(ErrorMessage = "Vui lòng mô tả tính cách / sức khỏe của bé!")]
        [StringLength(1000)]
        [Display(Name = "Mô tả về bé")]
        public string Description { get; set; }

        [StringLength(500)]
        [Display(Name = "Lý do gửi bé")]
        public string Reason { get; set; }
    }

    public class AdoptFormViewModel
    {
        public int CatId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên!")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        [RegularExpression(@"^(0|\+84)\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ!")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ!")]
        [StringLength(250)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [StringLength(500)]
        [Display(Name = "Lời nhắn / kinh nghiệm nuôi mèo")]
        public string Note { get; set; }
    }
}
