
using EFCoreSample.MySql.Data;
using EFCoreSample.MySql.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer; //JwtBearerDefaults
using Microsoft.AspNetCore.Authorization; //Authorize
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.MySql.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("[controller]")]
public class StudentController : ControllerBase
{
    private readonly ILogger<StudentController> _logger;
    private readonly ApiDbContext _dbContext;

    public StudentController(ILogger<StudentController> logger, ApiDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Student>))]
    public async Task<IActionResult> Get()
    {
        return Ok(await _dbContext.Students.ToListAsync());
    }

    [HttpGet("{rollNo}")]
    [ProducesResponseType(200, Type = typeof(Student))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByRollNo(int rollNo)
    {
        Student? existingStudent = await _dbContext.Students.FirstOrDefaultAsync(s => s.RollNo == rollNo);
        if (existingStudent is null)
            return NotFound();

        return Ok(existingStudent);
    }

    [HttpPost]
    [ProducesResponseType(200, Type = typeof(Student))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Add(Student student)
    {
        if (student is null)
            return BadRequest();

        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();

        return Ok(student);
    }

    [HttpPatch]
    [ProducesResponseType(200, Type = typeof(Student))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Student student)
    {
        if (student is null)
            return BadRequest();

        Student? existingStudent = await _dbContext.Students.FirstOrDefaultAsync(s => (s.Name == student.Name));
        if (existingStudent is null)
            return NotFound();

        existingStudent.Class = student.Class;
        existingStudent.Division = student.Division;
        existingStudent.RollNo = student.RollNo;

        await _dbContext.SaveChangesAsync();

        return Ok(existingStudent);
    }

    [HttpDelete]
    [ProducesResponseType(200, Type = typeof(Student))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delegate(Student student)
    {
         if (student is null)
            return BadRequest();

        Student? existingStudent = await _dbContext.Students.FirstOrDefaultAsync(s => (s.Name == student.Name));
        if (existingStudent is null)
            return NotFound();

        _dbContext.Students.Remove(existingStudent);
        await _dbContext.SaveChangesAsync();    

        return Ok(existingStudent);
    }
}