﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClubsCore.Contracts;
using ClubsCore.Mapping;
using ClubsCore.Mapping.DTO;
using ClubsCore.Models;
using ClubsCore.Paging;
using Contracts;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsCore.Controllers
{
    public class ClubsController : ApiControllerBase
    {
        public ClubsController(ClubsContext context, IMapper mapper, ILoggerManager logger, IRepositoryWrapper repository)
            : base(context, mapper, logger, repository)
        {
        }

        /// <summary>
        /// ClubDTO
        /// </summary>
        [Route("Get_Using_ClubDTO")]
        [HttpGet]
        public IActionResult Get_Clubs_With_ClubDTO([FromQuery] QueryParameters queryparameters)
        {
            var clubs_withClubDTOQuery = _context.Clubs
                                                 .OrderBy(c => c.Id);
            var clubs_withClubsDTO = Paginate<ClubDTO>(clubs_withClubDTOQuery, queryparameters);
            return Ok(clubs_withClubsDTO);
        }

        /// <summary>
        /// Get_All_UsingClubListingDTO
        /// </summary>
        [Route("Get_Using_ClubListingDTO")]
        [HttpGet]
        public IActionResult Get_Clubs_With_ClubListingDTO([FromQuery] QueryParameters queryparameters)
        {
            var clubs_withListingDTOQuery = _context.Clubs
                                      .OrderBy(c => c.Id);

            var clubs_withListingDTO = Paginate<ClubListingDTO>(clubs_withListingDTOQuery, queryparameters);
            return Ok(clubs_withListingDTO);
        }

        /// <summary>
        /// GetAll
        /// </summary>
        [HttpGet]
        public IActionResult GetClubs([FromQuery] QueryParameters queryparameters)
        {
            var clubsQuery = _context.Clubs
                                     .OrderBy(c => c.Id);

            var clubs = Paginate<ClubListingDTO>(clubsQuery, queryparameters);

            return Ok(clubs);
        }

        public List<TDto> Paginate<TDto>(IQueryable query, QueryParameters queryparameters)
        {
            return query.ProjectTo<TDto>(_mapper.ConfigurationProvider)
                        .Skip((queryparameters.PageNumber - 1) * queryparameters.PageSize)
                        .Take(queryparameters.PageSize)
                        .ToList();
        }

        /// <summary>
        /// Post
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostClubAsync(Club clubPost)
        {
            var post_club = _context.Clubs
                                    .Add(clubPost);

            await _context.SaveChangesAsync();
            return CreatedAtRoute("Post", new { Id = clubPost.Id }, clubPost);
        }

        /// <summary>
        /// Get_By_Id
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetClub(int id)
        {
            var club = _context.Clubs
                               .Where(x => x.Id == id)
                               .ProjectTo<ClubDTO>(_mapper.ConfigurationProvider)
                               .FirstOrDefault();

            if (club == null)
                return NotFound();

            return Ok(club);
        }

        /// <summary>
        /// Delete
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Club>> DeleteClub(int id)
        {
            var club = await _context.Clubs
                                     .FindAsync(id);
            if (club == null)
                return NotFound();

            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();

            return club;
        }

        /// <summary>
        /// Patch
        /// </summary>
        [HttpPatch]
        public IActionResult JsonPatchWithModelState(
            [FromBody] JsonPatchDocument<Club> patchDoc)
        {
            if (patchDoc != null)
            {
                var club = UpdateClub();

                patchDoc.ApplyTo(club, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return new ObjectResult(club);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        private Club UpdateClub()
        {
            return new Club
            {
                Id = 2,
                Type = "Sport",
                Name = "Hokkey Sharks"
            };
        }

        private bool ClubExists(int clubId, int studentId)
        {
            return _context.Clubs.Any(e => e.Id == clubId) && _context.Students.Any(p => p.Id == studentId);
        }
    }
}