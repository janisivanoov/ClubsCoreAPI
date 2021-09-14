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
        protected object _query;
        protected object _queryparameters;

        public ApiControllerBase(ClubsContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public List<TDto> Paginate<TDto>(IQueryable query, QueryParameters queryparameters)
        {
            return query.ProjectTo<TDto>(_mapper.ConfigurationProvider)
                        .Skip((queryparameters.PageNumber - 1) * queryparameters.PageSize)
                        .Take(queryparameters.PageSize)
                        .ToList();
        }

        public List<Student> Paginate<Student>(IOrderedQueryable<Student> query, QueryParameters queryparameters)
        {
            return query.ProjectTo<Student>(_mapper.ConfigurationProvider)
                                .Skip((queryparameters.PageNumber - 1) * queryparameters.PageSize)
                                .Take(queryparameters.PageSize)
                                .ToList();
        }
    }
}