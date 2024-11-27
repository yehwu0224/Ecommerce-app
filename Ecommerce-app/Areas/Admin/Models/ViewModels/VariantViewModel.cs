using Ecommerce_app.Models.Products;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_app.Areas.Admin.Models.ViewModels
{
    public class VariantViewModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("產品編號")]
        public int ProductId { get; set; }

        [DisplayName("產品名稱")]
        public string? Product { get; set; }

        [Required]
        [DisplayName("SKU編號")]
        public string SKU { get; set; } = "new SKU";

        [DisplayName("圖檔")]
        public byte[]? Image { get; set; }

        [DisplayName("庫存")]
        public int Stock { get; set; }

        [DisplayName("規格")]
        public string? Options { get; set; }
    }
}
