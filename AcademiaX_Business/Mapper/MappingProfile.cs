using AcademiaX_Business.Dtos;
using AcademiaX_Data_Access.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Mapper
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<RegisterRequestDTO, ApplicationUser>().ReverseMap();
		}
	}
}
