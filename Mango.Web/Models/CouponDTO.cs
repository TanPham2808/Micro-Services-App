﻿namespace Mango.Web.Models
{
    public class CouponDTO
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public int MinAmout { get; set; }
    }
}
