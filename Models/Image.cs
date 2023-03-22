using System.ComponentModel.DataAnnotations;

namespace ImagesApi.Models;

public class Image
{
    public int Id {get; set;}
    [Required]
    public string Name {get; set;}
    [Required]
    public string Description{get; set;}
    public string TimeCreated{get; set;}
    public string ImageLink{get; set;}
}