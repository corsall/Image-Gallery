using ImagesApi.Entities;
using ImagesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImagesApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ImagesController : Controller
{
    private readonly ImagesDbContext imagesDbContext; 
    public ImagesController(ImagesDbContext imagesDbContext)
    {
        this.imagesDbContext = imagesDbContext;
    }

    [HttpGet]

    public async Task<IActionResult> GetAllImages()
    {
        return Ok(await imagesDbContext.Images.ToListAsync());
    }

    [HttpGet("{id}")]

    public async Task<IActionResult> GetImageByID(int id)
    {
        var image = imagesDbContext.Images.FirstOrDefaultAsync(x => x.Id == id);

        if(image != null)
        {
            return Ok(await image);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost]

    public async Task<IActionResult> AddImage(Image image)
    {

        image.TimeCreated = DateTime.Now.ToShortTimeString();
        await imagesDbContext.Images.AddAsync(image);
        await imagesDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(AddImage), new {id = image.Id}, image);

    }

    [HttpPost("Upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if(file == null || file.Length == 0)
        {
            return BadRequest("Please select correct file to upload");
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return Json("/Images/" + file.FileName);
    }

    [HttpPut("{id}")]

    public async Task<IActionResult> UpdateNote(int id, Image newImage)
    {
        Image? existingImage = await imagesDbContext.Images.FirstOrDefaultAsync(x => x.Id == id);

        if(existingImage == null)
        {
            return NotFound();
        }

        existingImage.Name = newImage.Name;
        existingImage.ImageLink = newImage.ImageLink;
        existingImage.TimeCreated = newImage.TimeCreated;
        existingImage.Description = newImage.Description;

        await imagesDbContext.SaveChangesAsync();

        return Ok(existingImage);
    }

    [HttpDelete("{id}")]

    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await imagesDbContext.Images.FindAsync(id);

        if(image == null)
            NotFound();

        imagesDbContext.Remove(image!);
        await imagesDbContext.SaveChangesAsync();

        return Ok();
    }
}
