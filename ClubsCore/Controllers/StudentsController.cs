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
        /// StudentDTO in controller
        /// </summary>
        [HttpGet]
        public IQueryable<StudentDTO> GetStudent()
        {
            var students = from Student in _context.Students
                           select new StudentDTO()
                           {
                               Id = Student.Id,
                               FirstName = Student.FirstName,
                               LastName = Student.LastName,
                               BirthDate = Student.BirthDate
                           };
            return students;
        }

        /// <summary>
        /// GetAll
        /// </summary>
        [HttpGet]
        public IActionResult GetStudents([FromQuery] QueryParameters queryparameters)
        {
            var studentsQuery = _context.Students
                                     .OrderBy(c => c.Id);

            var students = Paginate(studentsQuery, queryparameters);

            return Ok(students);
        }

        /// <summary>
        /// Get_By_Id
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            var student = _context.Students
                               .Where(x => x.Id == id)
                               .ProjectTo<Student>(_mapper.ConfigurationProvider)
                               .FirstOrDefault();

            if (student == null)
                return NotFound();

            return Ok(student);
        }

        /// <summary>
        /// Post
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostStudentAsync(Student studentPost)
        {
            var post_student = _context.Students
                                    .Add(studentPost);

            await _context.SaveChangesAsync();
            return CreatedAtRoute("Post", new { Id = studentPost.Id }, studentPost);
        }

        /// <summary>
        /// Delete
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> DeleteStudent(int id)
        {
            var student = await _context.Students
                                     .FindAsync(id);
            if (student == null)
                return NotFound();

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return student;
        }

        /// <summary>
        /// Patch
        /// </summary>
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<Student> value)
        {
            try
            {
                var result = _context.Students.FirstOrDefault(n => n.Id == id);
                if (result == null)
                {
                    return NotFound();
                }
                value.ApplyTo(result, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);  //update database, if it was successful apply
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}