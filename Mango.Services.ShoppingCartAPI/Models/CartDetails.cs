using Mango.Services.ShoppingCartAPI.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models
{
    public class CartDetails
    {
        [Key]
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }

        // Tạo khóa ngoại
        [ForeignKey(nameof(CartHeaderId))]
        public CartHeader CartHeader { get; set; }

        public int ProductId { get; set; }
        [NotMapped] // Không thêm column vào table
        public ProductDTO Product { get; set; }
        public int Count { get; set; }
    }
}
