namespace SpotFinder.SearchService.Data.Entities;

public class ConceptTagReadModel
{
    public int Id { get; set; }
    public string NameTr { get; set; } = null!;
    public string NameEn { get; set; } = null!;
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
}
