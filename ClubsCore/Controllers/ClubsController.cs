﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClubsCore.Mapping;
using ClubsCore.Mapping.DTO;
using ClubsCore.Models;
using ClubsCore.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsCore.Controllers
{
    public class ClubsController : ApiControllerBase
    {
        private DbContextOptions options;

        public ClubsController(ClubsContext context, IMapper mapper)
            : base(context, mapper)
        {
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
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<Club> value)
        {
            try
            {
                var result = _context.Clubs.FirstOrDefault(n => n.Id == id);
                if (result == null)
                {
                    return BadRequest();
                }
                value.ApplyTo(result, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}