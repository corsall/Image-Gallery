using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    private readonly IMapper _mapper;
    public ImagesController(ImagesDbContext imagesDbContext, IMapper mapper)
    {
        _mapper = mapper;
        this.imagesDbContext = imagesDbContext;
    }

    //Returns all images
    [HttpGet("GetAll")]
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

    [HttpGet]
    public async Task<ActionResult<PagedResult<Image>>> GetPagedImages([FromQuery] QueryParameters queryParameters)
    {
        int totalSize = await imagesDbContext.Images.CountAsync();
        var items = await imagesDbContext.Images.Skip(queryParameters.PageSize * queryParameters.PageNumber).Take(queryParameters.PageSize).ToListAsync();
        var pagedImagesResult = new PagedResult<Image>
        {
            Items = items,
            PageNumber = queryParameters.PageNumber,
            RecordSize = queryParameters.PageSize,
            TotalCount = totalSize
        };

        if(pagedImagesResult == null)
        {
            return NotFound();
        }
        return Ok(pagedImagesResult);
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
    public async Task<ActionResult<Image>> AddImage(CreateImageDto createImage)
    {
        var image = _mapper.Map<Image>(createImage);
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
