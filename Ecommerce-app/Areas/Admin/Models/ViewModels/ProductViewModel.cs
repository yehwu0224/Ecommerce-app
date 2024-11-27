using Ecommerce_app.Models.Products;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_app.Areas.Admin.Models.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [DisplayName("產品名稱")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [DisplayName("產品描述")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [DisplayName("產品內容")]
        [DataType(DataType.MultilineText)]
        public string? Content { get; set; }

        [DisplayName("價格")]
        [DataType(DataType.Text)]
        public int Price { get; set; }

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
