using System;
using AutoMapper.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;

namespace VMS.Data
{
    public class MappingProfile:MapperConfigurationExpression
    {
        public MappingProfile() 
        {
            CreateMap<MCustomerDetailDto, MCustomerDetailExt>().ReverseMap();
            //CreateMap<MCustomerDetailExt, MCustomerDetailDto>().ReverseMap();

            CreateMap<VoucherDetailDto, VoucherDetailExt>().ReverseMap();
            //CreateMap<VoucherDetailExt, VoucherDetailDto>().ReverseMap();

            CreateMap<MPriceListVoucherDto, MPriceListVoucherExt>().ReverseMap();
            //CreateMap<MPriceListVoucherExt, MPriceListVoucherDto>().ReverseMap();

            CreateMap<ListPageDetail, ListPage>()
                .ForMember(d => d.Keyword, o => o.MapFrom(s => s.Keyword))
                .ForMember(d => d.fields, o => o.MapFrom(s => s.fields))
                .ForMember(d => d.PageSize, o => o.MapFrom(s => s.PageSize))
                .ForMember(d => d.PageNumber, o => o.MapFrom(s => s.PageNumber))
                .ReverseMap();
            //CreateMap<ListPage, ListPageDetail>()
            //    .ForMember(d => d.Keyword, o => o.MapFrom(s => s.Keyword))
            //    .ForMember(d => d.fields, o => o.MapFrom(s => s.fields))
            //    .ForMember(d => d.PageSize, o => o.MapFrom(s => s.PageSize))
            //    .ForMember(d => d.PageNumber, o => o.MapFrom(s => s.PageNumber))
            //    .ReverseMap();

            CreateMap<SystemConfigExt, SystemConfigParm>()
                .ForMember(d=> d.Id, o => o.MapFrom(s=> s.Id))
                .ForMember(d=> d.Name, o => o.MapFrom(s=> s.Name))
                .ForMember(d=> d.SystemCategory, o => o.MapFrom(s=> s.SystemCategory))
                .ForMember(d=> d.SystemSubCategory, o => o.MapFrom(s=> s.SystemSubCategory))
                .ForMember(d=> d.SystemCode, o => o.MapFrom(s=> s.SystemCode))
                .ForMember(d=> d.SystemValue, o => o.MapFrom(s=> s.SystemValue))
                .ForMember(d=> d.Description, o => o.MapFrom(s=> s.Description))
                .ReverseMap();
            //CreateMap<SystemConfigParm, SystemConfigExt>()
            //    .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            //    .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            //    .ForMember(d => d.SystemCategory, o => o.MapFrom(s => s.SystemCategory))
            //    .ForMember(d => d.SystemSubCategory, o => o.MapFrom(s => s.SystemSubCategory))
            //    .ForMember(d => d.SystemCode, o => o.MapFrom(s => s.SystemCode))
            //    .ForMember(d => d.SystemValue, o => o.MapFrom(s => s.SystemValue))
            //    .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
            //    .ReverseMap();
        }
    }
}
