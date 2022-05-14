using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [ApiController]
    [Route("students")]
    [EnableCors]
    public class StudentController : Controller
    {
        private readonly DataDbContext dbContext;

        public StudentController(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// List students
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Student[]>> List()
        {
            return await dbContext.Students.ToArrayAsync();
        }

        /// <summary>
        /// Get a student by ID
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Student>> Get(int id)
        {
            var student = await dbContext.Students.FirstOrDefaultAsync(s => s.ID == id);
            return student == null ? NotFound() : student;
        }

        /// <summary>
        /// Replace a student by ID. Must provide all data
        /// </summary>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<Student>> Replace(int id, [FromBody] Student student)
        {
            if (!ModelState.IsValid) return BadRequest();

            var studentInDb = await dbContext.Students.FirstOrDefaultAsync(s => s.ID == id);
            if (studentInDb == null) return NotFound();

            dbContext.Entry(studentInDb).State = EntityState.Detached;
            student.ID = id;
            dbContext.Students.Attach(student);
            dbContext.Entry(student).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();
            return student;
        }
        
        /// <summary>
        /// Create a student
        /// </summary>
        [HttpPost]
        [Route("{id:int}")]
        public async Task<ActionResult<Student>> Create(int id, [FromBody] Student student)
        {
            if (!ModelState.IsValid) return BadRequest();
            
            var studentInDb = await dbContext.Students.FirstOrDefaultAsync(s => s.ID == id);
            if (studentInDb != null) return Conflict();

            student.ID = id;
            await dbContext.Students.AddAsync(student);
            await dbContext.SaveChangesAsync();
            return student;
        }
        
        /// <summary>
        /// Delete a student
        /// </summary>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<Student>> Delete(int id)
        {
            var student = await dbContext.Students.FirstOrDefaultAsync(s => s.ID == id);
            if (student == null) return NotFound();
            dbContext.Students.Remove(student);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}