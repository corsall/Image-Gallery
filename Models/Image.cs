namespace ImagesApi.Models;

public class Image
{
    public int Id {get; set;}
    public string? Name {get; set;}
    public string? Description{get; set;}
    public string? TimeCreated{get; set;}
    public string? ImageLink{get; set;}
}