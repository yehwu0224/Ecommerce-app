using Ecommerce_app.Models.Products;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_app.Areas.Admin.Models.ViewModels
{
    public class OptionViewModel
    {
        public int Id { get; set; }

        [DisplayName("規格名稱")]
        public string? Type { get; set; }

        [DisplayName("建立日期")]
        [DataType(DataType.DateTime)]
        public DateTime Created_at { get; set; }

        [DisplayName("最後修改日期")]
        [DataType(DataType.DateTime)]
        public DateTime Modiftied_at { get; set; }

        public virtual List<OptionValueViewModel> optionValuesVM { get; set; } = [];
    }

}
