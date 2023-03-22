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

    //Returns all images
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Image>>> GetAllImages()
    {
        var images = await imagesDbContext.Images.ToListAsync();
        if(images == null)
        {
            return NotFound();
        }
        return Ok(images);
    }

    //Returns single image
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Image>> GetImageByID(int id)
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

    //This endpoint establishes only a description of the image and a link to that image, if such link exists.
    [HttpPost]
    public async Task<ActionResult<Image>> AddImage(Image image)
    {
        image.TimeCreated = DateTime.Now.ToShortTimeString();
        await imagesDbContext.Images.AddAsync(image);
        await imagesDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(AddImage), new {id = image.Id}, image);
    }

    //This endpoint is used when the user has selected an image to be uploaded.
    //TODO: Зроби обмеження по розміру на зберігання файлу
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

    //This endpoint updates existing image.
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Image>> UpdateNote(int id, Image newImage)
    {
        if(id != newImage.Id)
        {
            return BadRequest();
        }
        Image existingImage = await imagesDbContext.Images.FirstOrDefaultAsync(x => x.Id == id);

        if(existingImage == null)
        {
            return NotFound();
        }

        existingImage.Name = newImage.Name;
        existingImage.ImageLink = newImage.ImageLink;
        existingImage.TimeCreated = newImage.TimeCreated;
        existingImage.Description = newImage.Description;

        try
        {
            imagesDbContext.Update(existingImage);
            await imagesDbContext.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            if(!(await imagesDbContext.Images.AnyAsync(x => x.Id == id)))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Ok(existingImage);
    }

    [HttpDelete("{id}")]

    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await imagesDbContext.Images.FindAsync(id);

        if(image == null)
            NotFound();

        imagesDbContext.Remove(image);
        await imagesDbContext.SaveChangesAsync();

        return NoContent();
    }
}
