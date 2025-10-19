namespace TekramApp.ViewModels
{
    public class CountryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }
    }
}
