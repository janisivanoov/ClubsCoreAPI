using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClubsCore.Mapping;
using ClubsCore.Mapping.DTO;
using ClubsCore.Models;
using ClubsCore.Paging;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
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
        /// Filter by name
        /// </summary>
        [HttpGet]
        public IActionResult GetClubWithFilter([FromQuery] QueryParameters queryParameters)
        {
            using (var context = new ClubsContext(options))
            {
                var clubs_from_context = from Student in _context.Students where Student.FirstName.StartsWith("A") select Student;
                var club = context.Clubs.Where(club => club.Name == "Ali")
                                              .OrderBy(c => c.Id)
                                              .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                                              .Take(queryParameters.PageSize)
                                              .ToList();
                return Ok(club);
            }
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
    }
}