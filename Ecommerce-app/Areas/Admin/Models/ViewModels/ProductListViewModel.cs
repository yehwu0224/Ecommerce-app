using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Ecommerce_app.Models.Products;

namespace Ecommerce_app.Areas.Admin.Models.ViewModels
{
    public class ProductListViewModel
    {
        public int Id { get; set; }

        [DisplayName("產品名稱")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [DisplayName("分類")]
        [DataType(DataType.Text)]
        public string? Department { get; set; }

        [DisplayName("子類別")]
        [DataType(DataType.Text)]
        public string? Category { get; set; }

        [DisplayName("SKU")]
        [DataType(DataType.Text)]
        public int Count { get; set; }

        [DisplayName("圖檔")]
        public string? ImageStr { get; set; }

        [DisplayName("產品建立日期")]
        [DataType(DataType.DateTime)]
        public DateTime Created_at { get; set; }

        [DisplayName("最後修改日期")]
        [DataType(DataType.DateTime)]
        public DateTime Modiftied_at { get; set; }

    }
}
