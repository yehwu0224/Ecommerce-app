using System.ComponentModel.DataAnnotations;

namespace Ecommerce_app.Models.Orders
{
    public class Order : BaseEntity
    {
        // public int Id { get; set; }
        [Key]
        public string? OrderId { get; set; }

        public string? OrderDate { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int TotalAmount { get; set; }

        [Required]
        public string? ReceiverName { get; set; }
        [Required]
        public string? ReceiverAdress { get; set; }
        [Required]
        public string? ReceiverPhone { get; set; }

        public string? PaymentType { get; set; }
        public int? Status { get; set; }
        public string? CheckMacValue { get; set; }

        //public string? OrderItem { get; set; }
        public virtual List<OrderItem>? OrderItems { get; set; }
    }
}
