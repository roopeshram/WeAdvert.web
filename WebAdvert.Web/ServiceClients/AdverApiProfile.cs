﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;

namespace WebAdvert.Web.ServiceClients
{
    public class AdverApiProfile : Profile
    {
        public AdverApiProfile()
        {
            CreateMap<AdvertModel, CreateAdvertModel>().ReverseMap();
            CreateMap<CreateAdvertResponse, AdvertResponse>().ReverseMap();
            CreateMap<ConfirmAdvertRequest,ConfirmAdvertModel >().ReverseMap();

        }
    }
}
