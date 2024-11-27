using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_app.Areas.Admin.Models.ViewModels
{
    public class OptionValueViewModel
    {
        public int Id { get; set; }

        [DisplayName("名稱")]
        public string? Value { get; set; }

        [DisplayName("圖檔")]
        public string? Image { get; set; }

        [DisplayName("建立日期")]
        [DataType(DataType.DateTime)]
        public DateTime Created_at { get; set; }

        [DisplayName("最後修改日期")]
        [DataType(DataType.DateTime)]
        public DateTime Modiftied_at { get; set; }
    }
}
