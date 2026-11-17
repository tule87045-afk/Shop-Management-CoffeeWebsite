
using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm!")]
        [Display(Name = "Tên sản phẩm")]// đây là code mới thêm vào
        public string Name { get; set; }

        public string Category { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm!")]
        [Range(1000, 10000000, ErrorMessage = "Giá sản phẩm phải từ 1,000đ trở lên!")]
        [Display(Name = "Giá bán")]// mới thêm vào
        public decimal Price { get; set; }
        [Display(Name = "Hình ảnh")]// mới thêm
        public string ImageUrl { get; set; }
        [Display(Name = "Mô tả")]// mới thêm
        public string Description { get; set; }

        public string Size { get; set; }

        public string Ingredients { get; set; }

        public string PreparationTime { get; set; }

        public string Note { get; set; }

    }
}
