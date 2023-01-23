using System.ComponentModel.DataAnnotations;
namespace EFCoreSample.MySql.Models;

public class Teacher
{
    public string? Name { get; set; }
    [Key]
    public int PayrollNo { get; set; }
    public List<Student>? Students { get; set; }
}