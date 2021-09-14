using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClubsCore.Paging;
using System.Collections.Generic;
using System.Linq;

namespace ClubsCore.Controllers
{
    public class Paginate
    {
        protected object _query;
        protected object _queryparameters;
        protected IMapper _mapper;

        public List<TDto> Paginate1<TDto>(IQueryable query, QueryParameters queryparameters)
        {
            return query.ProjectTo<TDto>(_mapper.ConfigurationProvider)
                        .Skip((queryparameters.PageNumber - 1) * queryparameters.PageSize)
                        .Take(queryparameters.PageSize)
                        .ToList();
        }
    }
}