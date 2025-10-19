namespace TekramApp.ViewModels
{
    public class ShopDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public bool IsOpen { get; set; }

        public string Currency { get; set; } = null!;
    }

    public class ShopCreateDto
    {
        public string Name { get; set; } = null!;
        public bool IsOpen { get; set; }
        public string Currency { get; set; } = null!;
        public IFormFile? Image { get; set; }
    }
}
