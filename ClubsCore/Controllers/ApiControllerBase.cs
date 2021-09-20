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

        public List<TDto> Paginate<TDto>(IQueryable query, QueryClubParameters QueryClubParameters)
        {
            return query.ProjectTo<TDto>(_mapper.ConfigurationProvider)
                        .Skip((QueryClubParameters.PageNumber - 1) * QueryClubParameters.PageSize) //Skippin adding process PageNumber and PageSize
                        .Take(QueryClubParameters.PageSize) //Using PageSize
                        .ToList(); //Sending to List
        }

        public List<TDTO> Paginate<TDTO>(IQueryable<TDTO> query, QueryStudentParameters QueryStudentParameters)
        {
            return query.ProjectTo<TDTO>(_mapper.ConfigurationProvider)
                                .Skip((QueryStudentParameters.PageNumber - 1) * QueryStudentParameters.PageSize) //Skipping adding process PageNumber and PageSize
                                .Take(QueryStudentParameters.PageSize) //Using PageSize
                                .ToList(); //Sending to List
        }
    }
}