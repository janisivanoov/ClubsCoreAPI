using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClubsCore.Mapping.DTO;
using ClubsCore.Models;
using ClubsCore.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsCore.Controllers
{
    public class StudentsController : ApiControllerBase
    {
        public StudentsController(ClubsContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        /// <summary>
        /// GetAll
        /// </summary>
        [HttpGet]
        public ActionResult GetStudents([FromQuery] QueryStudentParameters queryparameters, string FirtsName = "Ali")
        {
            var studentsQuery = _context.Students
                                     .OrderBy(c => c.Id); //ordering all students by Id

            bool applyingFilter = true;

            if (false == applyingFilter) //using bool for filter
            {
                var filterForStudents = _context.Students
                                                .Where(n => n.FirstName == FirtsName) //all Students with name "Ali"
                                                .ToList(); //sent to list
                if (filterForStudents == null)
                    return NotFound();
            }

            var students = Paginate(studentsQuery, queryparameters); //using Paginate

            return Ok(students);
        }

        /// <summary>
        /// Get_By_Id
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult GetStudent(int id, string name)
        {
            var student = _context.Students
                               .Where(x => x.Id == id) //searching for Student using Id
                               .ProjectTo<StudentDTO>(_mapper.ConfigurationProvider)//using mapper and StudentDTO
                               .FirstOrDefault(); //Selecting the club by Id with StudentDTO parameters or make it as a default

            if (student == null)
                return NotFound();

            return Ok(student);
        }

        /// <summary>
        /// Post
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> PostStudentAsync(Student studentPost)
        {
            var post_student = _context.Students
                                    .Add(studentPost); //using Add function to post a student

            await _context.SaveChangesAsync(); //saving edit in a database
            return CreatedAtRoute("Post", new { Id = studentPost.Id }, studentPost);
        }

        /// <summary>
        /// Delete
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> DeleteStudent(int id)
        {
            var student = await _context.Students
                                     .FindAsync(id); //searching for student using Id
            if (student == null)
                return NotFound();

            _context.Students.Remove(student); //using Remove function
            await _context.SaveChangesAsync(); //saving all in database

            return student;
        }

        /// <summary>
        /// Patch
        /// </summary>
        [HttpPatch("{id}")]
        public ActionResult Patch(int id, [FromBody] JsonPatchDocument<Student> value)
        {
            try
            {
                var result = _context.Students.FirstOrDefault(n => n.Id == id); //Getting Student by Id

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