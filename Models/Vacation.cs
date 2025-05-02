using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace VacationManager.Models
{
    public class Vacation
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        public int DurationDays { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        [ValidateNever] // ✅ this fixes the issue
        public Employee Employee { get; set; }
    }
}
