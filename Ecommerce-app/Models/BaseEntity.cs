namespace Ecommerce_app.Models
{
    public abstract class BaseEntity
    {
        public DateTime Created_at { get; set; }
        public DateTime Modiftied_at { get; set; }
    }
}
