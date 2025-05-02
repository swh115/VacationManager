using System.ComponentModel.DataAnnotations;
namespace VacationManager.Models;

public class Employee
{
    public int Id { get; set; }

    [Required]
    public string? username { get; set; }
    public string? password { get; set; }

}