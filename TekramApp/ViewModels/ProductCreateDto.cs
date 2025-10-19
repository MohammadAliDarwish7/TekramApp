namespace TekramApp.ViewModels
{
    public class ProductCreateDto
    {
        public Guid ShopId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool Availability { get; set; }
        public IFormFile? Image { get; set; }
    }
}
