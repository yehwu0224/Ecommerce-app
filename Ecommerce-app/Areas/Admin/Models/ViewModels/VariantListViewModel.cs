using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_app.Areas.Admin.Models.ViewModels
{
    public class VariantListViewModel
    {
        public int Id { get; set; }

        [DisplayName("產品名稱")]
        public string? Product { get; set; }

        [DisplayName("SKU編號")]
        public string SKU { get; set; } = "new SKU";

        [DisplayName("圖檔")]
        public string? Image { get; set; }

        [DisplayName("庫存")]
        public int Stock { get; set; }

        [DisplayName("產品建立日期")]
        [DataType(DataType.DateTime)]
        public DateTime Created_at { get; set; }

        [DisplayName("最後修改日期")]
        [DataType(DataType.DateTime)]
        public DateTime Modiftied_at { get; set; }
    }
}
