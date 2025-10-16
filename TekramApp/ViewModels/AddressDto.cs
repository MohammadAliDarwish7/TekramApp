namespace TekramApp.ViewModels
{
    public class AddressDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string AddressLine { get; set; }
        //public Guid CountryId { get; set; }
        //public Guid CityId { get; set; }
        public bool IsDefault { get; set; }
    }
}
