using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ImagesApi.Models;

namespace ImagesApi.Configs
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Image, CreateImageDto>().ReverseMap();
        }
    }
}