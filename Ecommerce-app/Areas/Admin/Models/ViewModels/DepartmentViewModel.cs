using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_app.Areas.Admin.Models.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [DisplayName("類別名稱")]
        public string? Name { get; set; }

        [DisplayName("建立日期")]
        [DataType(DataType.DateTime)]
        public DateTime Created_at { get; set; }

        [DisplayName("最後修改日期")]
        [DataType(DataType.DateTime)]
        public DateTime Modiftied_at { get; set; }
        
        public virtual List<CategoryViewModel>? CategoriesVM { get; set; }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        
        [DisplayName("子類別名稱")]
        public string? Name { get; set; }

        [DisplayName("建立日期")]
        [DataType(DataType.DateTime)]
        public DateTime Created_at { get; set; }

        [DisplayName("最後修改日期")]
        [DataType(DataType.DateTime)]
        public DateTime Modiftied_at { get; set; }
    }
}
