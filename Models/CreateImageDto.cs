using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


public class CreateImageDto
{
    [Required]
    public string Name {get; set;}
    [Required]
    public string Description{get; set;}
    [Required]
    public string ImageLink{get; set;}
}
