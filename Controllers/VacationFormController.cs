using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VacationManager.Data;
using VacationManager.Models;
using Microsoft.EntityFrameworkCore;

namespace VacationManager.Controllers
{
    [Authorize]
    public class VacationFormController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VacationFormController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(); // Views/VacationForm/Add.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Add(Vacation vacation)
        {
            if (!ModelState.IsValid)
            {
                return View(vacation);
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.username == User.Identity.Name);
            if (employee == null)
                return Unauthorized();

            vacation.EmployeeId = employee.Id;

            _context.Vacations.Add(vacation);
            Console.WriteLine("About to insert vacation");

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vacation = await _context.Vacations.FindAsync(id);
            if (vacation == null) return NotFound();
            return View(vacation);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Vacation updated)
        {
            if (!ModelState.IsValid) return View(updated);

            var vacation = await _context.Vacations.FindAsync(updated.Id);
            if (vacation == null) return NotFound();

            vacation.Description = updated.Description;
            vacation.FromDate = updated.FromDate;
            vacation.ToDate = updated.ToDate;
            vacation.DurationDays = updated.DurationDays;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }




    }
}
