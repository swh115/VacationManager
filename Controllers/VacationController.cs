using Microsoft.AspNetCore.Mvc;
using VacationManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace VacationManager.Controllers
{
    [Authorize]
    [Route("api/vacations")]
    public class VacationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VacationController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("my")]
        public async Task<IActionResult> GetMyVacations(int page = 1, int rows = 10)
        {
            var username = User.Identity?.Name;

            var query = _context.Vacations
                .Include(v => v.Employee)
                .Where(v => v.Employee.username == username);

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / rows);

            var vacations = await query
                .OrderByDescending(v => v.FromDate)
                .Skip((page - 1) * rows)
                .Take(rows)
                .Select(v => new
                {
                    v.Id,
                    v.Description,
                    v.FromDate,
                    v.ToDate,
                    v.DurationDays
                })
                .ToListAsync();

            return Json(new
            {
                page,
                total = totalPages,
                records = totalRecords,
                rows = vacations
            });
        }



    }
}
