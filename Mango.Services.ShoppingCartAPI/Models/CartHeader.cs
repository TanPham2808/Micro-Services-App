using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models
{
    public class CartHeader
    {
        [Key]
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }

        [NotMapped] // Ko tạo coloumn trong table
        public double Discount { get; set; }
        [NotMapped] // Ko tạo coloumn trong table
        public double CartTotal { get;}

    }
}
