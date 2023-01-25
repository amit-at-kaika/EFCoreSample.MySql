using System.ComponentModel.DataAnnotations;
namespace EFCoreSample.MySql.Models;

public class Student
{
    public string? Name { get; set; }
    [Key]
    public int RollNo { get; set; }
    public int Class { get; set; }
    public char Division { get; set; }      
}