namespace SpotFinder.VenueService.Data.Entities;

public class District
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = "İstanbul";

    public ICollection<Venue> Venues { get; set; } = new List<Venue>();
}
