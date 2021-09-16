using AutoMapper;
using ClubsCore.Mapping.DTO;
using ClubsCore.Models;

namespace ClubsCore.Mapping
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, StudentDTO>();
        }
    }
}