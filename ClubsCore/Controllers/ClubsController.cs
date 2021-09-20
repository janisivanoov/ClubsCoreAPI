using AutoMapper;
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
        public ActionResult GetClubs([FromQuery] QueryClubParameters queryparameters, string Name = "Sport")
        {
            var clubsQuery = _context.Clubs
                                     .OrderBy(c => c.Id); //ordering all clubs by Id

            bool applyFilter = true;

            if (applyFilter == true) //using bool
            {
                var filterForClubs = _context.Clubs
                                     .Where(n => n.Name == Name) //all Clubs with name "Sport"
                                     .ToList(); //sent to list

                if (filterForClubs == null)
                    return NotFound();
            }

            var clubs = Paginate<ClubListingDTO>(clubsQuery, queryparameters); //using Paginate

            return Ok(clubs);
        }

        /// <summary>
        /// Get_By_Id
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult GetClub(int id, string name)
        {
            var club = _context.Clubs
                               .Where(x => x.Id == id) //searching for Club using Id
                               .ProjectTo<ClubDTO>(_mapper.ConfigurationProvider) //using mapper and ClubDTO
                               .FirstOrDefault(); //Selecting the club by Id with ClubDTO parameters or make it as a default

            if (club == null)
                return NotFound();

            return Ok(club);
        }

        /// <summary>
        /// Post
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> PostClubAsync(Club clubPost)
        {
            var post_club = _context.Clubs
                                    .Add(clubPost); //using Add function to post a club

            await _context.SaveChangesAsync(); //saving edit in a database
            return CreatedAtRoute("Post", new { Id = clubPost.Id }, clubPost);
        }

        /// <summary>
        /// Delete
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Club>> DeleteClub(int id)
        {
            var club = await _context.Clubs
                                     .FindAsync(id); //searching for club using Id
            if (club == null)
                return NotFound();

            _context.Clubs.Remove(club); //using Remove function
            await _context.SaveChangesAsync(); //saving all in database

            return club;
        }

        /// <summary>
        /// Patch
        /// </summary>
        [HttpPatch("{id}")]
        public ActionResult Patch(int id, [FromBody] JsonPatchDocument<Club> value)
        {
            try
            {
                var result = _context.Clubs.FirstOrDefault(n => n.Id == id); //Getting Club by Id

                if (result == null)
                    return NotFound();

                value.ApplyTo(result, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState); //result gets the values from the patch request

                _context.SaveChanges(); //Saving in database

                if (false == ModelState.IsValid)
                    return BadRequest();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex); //Catching exceptions
            }
        }
    }
}