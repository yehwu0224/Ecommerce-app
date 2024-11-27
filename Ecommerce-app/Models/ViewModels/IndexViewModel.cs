using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Ecommerce_app.Models.Products;

namespace Ecommerce_app.Models.ViewModels
{
    public class IndexViewModel
    {
        public int Id { get; set; }

        [DisplayName("產品名稱")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [DisplayName("產品描述")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [DisplayName("價格")]
        [DataType(DataType.Text)]
        public int Price { get; set; }

        [DisplayName("分類")]
        [DataType(DataType.Text)]
        public string? Department { get; set; }

        [DisplayName("子類別")]
        [DataType(DataType.Text)]
        public string? Category { get; set; }

        [DisplayName("圖檔")]
        public string? ImageStr { get; set; }

        [DisplayName("顏色")]
        public List<string>? Colors { get; set; }
    }
}
