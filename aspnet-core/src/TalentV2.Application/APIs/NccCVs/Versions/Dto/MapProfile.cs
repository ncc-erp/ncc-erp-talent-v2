using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace TalentV2.APIs.NccCVs.Versions.Dto
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<VersionDto, TalentV2.Entities.NccCVs.Versions>().ReverseMap();
        }
    }
}
