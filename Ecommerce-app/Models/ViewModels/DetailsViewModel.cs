using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ecommerce_app.Models.ViewModels
{
    public class DetailsViewModel
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

        [DisplayName("分類")]
        [DataType(DataType.Text)]
        public string? Department { get; set; }

        [DisplayName("子類別")]
        [DataType(DataType.Text)]
        public string? Category { get; set; }

        [DisplayName("圖檔")]
        public string? ImageStr { get; set; }

        public List<string>? Album { get; set; }

        public SKUViewModel? SKU { get; set; }

        public string? SKUNum { get; set; }

        public List<ColorModel>? Colors { get; set; }

    }

    public class ColorModel
    {
        public string? Value { get; set; }
        public string? ImageStr { get; set; }
    }

    public class SKUViewModel
    {
        public int Id { get; set; }

        [DisplayName("產品編號")]
        public int ProductId { get; set; }

        [DisplayName("產品名稱")]
        public string? Product { get; set; }

        [DisplayName("SKU編號")]
        public string? SKU { get; set; }

        [DisplayName("圖檔")]
        public byte[]? Image { get; set; }

        [DisplayName("庫存")]
        public int Stock { get; set; }

        [DisplayName("規格")]
        public string? Options { get; set; }
    }
}
