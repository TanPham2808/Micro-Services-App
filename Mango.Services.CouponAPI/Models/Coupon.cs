namespace Mango.Services.CouponAPI.Models
{
    public class Coupon
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public int MinAmout { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
