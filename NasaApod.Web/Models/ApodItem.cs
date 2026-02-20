namespace NasaApodMvc.Models;

public class ApodItem
{
    public int Id { get; set; }
    public DateOnly ApodDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public string Url { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public string? ServiceVersion { get; set; }
    public DateTime SavedAt { get; set; }
}
