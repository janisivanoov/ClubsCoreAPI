using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using System.Web.Http.Description;

namespace ClubsCore.Controllers
{
    public class StudentsController : ApiControllerBase
    {
        private readonly DbContextOptions options;

        public StudentsController(ClubsContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        /// <summary>
        /// Filter by name
        /// </summary>
        [HttpGet]
        public IActionResult GetStudentWithFilter([FromQuery] QueryParameters queryParameters)
        {
            using (var context = new ClubsContext(options))
            {
                var students_from_context = from Student in _context.Students where Student.FirstName.StartsWith("A") select Student;
                var student = context.Students.Where(students => students.FirstName == "Ali")
                                              .OrderBy(c => c.Id)
                                              .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                                              .Take(queryParameters.PageSize)
                                              .ToList();
                return Ok(student);
            }
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
        /// StudentDTO in controller with Id search
        /// </summary>
        [ResponseType(typeof(StudentDTO))]
        public async Task<System.Web.Http.IHttpActionResult> GetStudentUsingDTO(int id)
        {
            var student = await _context.Students
                                         .Include(student => student.FirstName)
                                         .Select(student => new StudentDTO()
                                         {
                                             Id = student.Id,
                                             FirstName = student.FirstName,
                                             LastName = student.LastName,
                                             BirthDate = student.BirthDate
                                         })
                                         .SingleOrDefaultAsync(student => student.Id == id);
            if (student == null)
            {
                return (System.Web.Http.IHttpActionResult)NotFound();
            }
            return (System.Web.Http.IHttpActionResult)Ok(student);
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