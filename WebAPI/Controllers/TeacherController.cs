
using EFCoreSample.MySql.Data;
using EFCoreSample.MySql.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.MySql.Controllers;

[ApiController]
[Route("[controller]")]
public class TeacherController : ControllerBase
{
    private readonly ILogger<TeacherController> _logger;
    private readonly ApiDbContext _dbContext;

    public TeacherController(ILogger<TeacherController> logger, ApiDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Teacher>))]
    public async Task<IActionResult> Get()
    {
        return Ok(await _dbContext.Teachers.ToListAsync());
    }

    [HttpGet("{payrollNo}")]
    [ProducesResponseType(200, Type = typeof(Teacher))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByPayrollNo(int payrollNo)
    {
        Teacher? existingTeacher = await _dbContext.Teachers.FirstOrDefaultAsync(s => s.PayrollNo == payrollNo);
        if (existingTeacher is null)
            return NotFound();

        return Ok(existingTeacher);
    }

    [HttpPost]
    [ProducesResponseType(200, Type = typeof(Teacher))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Add(Teacher teacher)
    {
        if (teacher is null)
            return BadRequest();

        _dbContext.Teachers.Add(teacher);
        await _dbContext.SaveChangesAsync();

        return Ok(teacher);
    }

    [HttpPatch]
    [ProducesResponseType(200, Type = typeof(Teacher))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Teacher teacher)
    {
        if (teacher is null)
            return BadRequest();

        Teacher? existingTeacher = await _dbContext.Teachers.FirstOrDefaultAsync(s => (s.Name == teacher.Name));
        if (existingTeacher is null)
            return NotFound();

        existingTeacher.PayrollNo = teacher.PayrollNo;
        existingTeacher.Students = teacher.Students;

        await _dbContext.SaveChangesAsync();

        return Ok(existingTeacher);
    }

    [HttpDelete]
    [ProducesResponseType(200, Type = typeof(Teacher))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delegate(Teacher teacher)
    {
         if (teacher is null)
            return BadRequest();

        Teacher? existingTeacher = await _dbContext.Teachers.FirstOrDefaultAsync(s => (s.Name == teacher.Name));
        if (existingTeacher is null)
            return NotFound();

        _dbContext.Teachers.Remove(existingTeacher);
        await _dbContext.SaveChangesAsync();    

        return Ok(existingTeacher);
    }
}