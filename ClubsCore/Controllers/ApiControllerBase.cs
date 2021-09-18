using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClubsCore.Models;
using ClubsCore.Paging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ClubsCore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected readonly IMapper _mapper;
        protected readonly ClubsContext _context;

        public ApiControllerBase(ClubsContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public List<TDto> Paginate<TDto>(IQueryable query, QueryClubParameters QueryClubParameters, ClubsContext clubsContext, string Name)
        {
            var NameCheck = _context.Clubs
                                    .Where(n => n.Name == Name)
                                    .ToList()
                                    .FirstOrDefault();

            //if(NameCheck == null)
            //return BadRequest(); ---- Consulte con Ali. No se puede convertir BadRequest() el error. Solucion dos

            return query.ProjectTo<TDto>(_mapper.ConfigurationProvider)
                        .Skip((QueryClubParameters.PageNumber - 1) * QueryClubParameters.PageSize)
                        .Take(QueryClubParameters.PageSize)
                        .ToList();
        }

        public List<Student> Paginate(IOrderedQueryable<Student> query, QueryStudentParameters QueryStudentParameters, ClubsContext clubsContext, string FirstName)
        {
            var studentNameCheck = _context.Students
                                           .Where(n => n.FirstName == FirstName)
                                           .ToList()
                                           .FirstOrDefault();

            //if (studentNameCheck == null)
            //return BadRequest();   //---- Consulte con Ali. No se puede convertir BadRequest() el error. Solucion dos

            return query.ProjectTo<Student>(_mapper.ConfigurationProvider)
                                .Skip((QueryStudentParameters.PageNumber - 1) * QueryStudentParameters.PageSize)
                                .Take(QueryStudentParameters.PageSize)
                                .ToList();
        }
    }
}