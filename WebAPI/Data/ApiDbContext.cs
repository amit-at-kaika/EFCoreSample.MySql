
using EFCoreSample.MySql.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.MySql.Data;

public class ApiDbContext : DbContext
{
    public DbSet<Student>? Students { get; set; }
    public DbSet<Teacher>? Teachers { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {

    }
}