
using EFCoreSample.MySql.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; //IdentityDbContext
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.MySql.Data;

public class ApiDbContext : IdentityDbContext
{
    public DbSet<Student>? Students { get; set; }
    public DbSet<Teacher>? Teachers { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {

    }
}