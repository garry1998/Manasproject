using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication26.Models;

namespace WebApplication26.Controllers
{
    public class AdminDetailsController : Controller
    {
        private readonly project1211Context _context;

        public AdminDetailsController(project1211Context context)
        {
            _context = context;
        }

        // GET: AdminDetails
        public async Task<IActionResult> Index()
        {
            var CurrentUserIDSession = HttpContext.Session.GetString("name");
            if (string.IsNullOrEmpty(CurrentUserIDSession))
            {

                return RedirectToAction("Index", "Login");

            }
            else
                return View(await _context.AdminDetails.ToListAsync());
        }

        // GET: AdminDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminDetail = await _context.AdminDetails
                .FirstOrDefaultAsync(m => m.PkAdminId == id);
            if (adminDetail == null)
            {
                return NotFound();
            }

            return View(adminDetail);
        }

        // GET: AdminDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PkAdminId,EmployeeId,FirstName,LastName,DateOfBirth,Email,Contact,Pswd,Address,FatherName,CreatedDate,IsActive,IsDeleted")] AdminDetail adminDetail)
        {
            if (ModelState.IsValid)
            {
                var ac = Convert.ToDateTime(adminDetail.DateOfBirth);

                DateTime PresentYear = DateTime.Now;
                TimeSpan ts = PresentYear - ac;
                if (ts.Days > 0)
                {
                    DateTime Age = DateTime.MinValue.AddDays(ts.Days);
                    if (Age.Year - 1 < 18)
                    {
                        ModelState.AddModelError("", "Age cant be less than 18");
                        return View(adminDetail);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dob cant be future date");
                    return View(adminDetail);
                }
                if (adminDetail.EmployeeId == null || adminDetail.Email == null || adminDetail.Pswd == null || adminDetail.FirstName == null) { return View(adminDetail); }
                var record_Check = _context.MstUsers.FirstOrDefault(m => m.Email == adminDetail.Email || m.Contact == adminDetail.Contact);
                if (record_Check != null)
                { ModelState.AddModelError("", "Email or Mobile Assosiated with other Account"); return View(adminDetail); }
                _context.Add(adminDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adminDetail);
        }

        // GET: AdminDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminDetail = await _context.AdminDetails.FindAsync(id);
            if (adminDetail == null)
            {
                return NotFound();
            }
            return View(adminDetail);
        }

        // POST: AdminDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PkAdminId,EmployeeId,FirstName,LastName,DateOfBirth,Email,Contact,Pswd,Address,FatherName,CreatedDate,IsActive,IsDeleted")] AdminDetail adminDetail)
        {
            if (id != adminDetail.PkAdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminDetailExists(adminDetail.PkAdminId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(adminDetail);
        }

        // GET: AdminDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminDetail = await _context.AdminDetails
                .FirstOrDefaultAsync(m => m.PkAdminId == id);
            if (adminDetail == null)
            {
                return NotFound();
            }

            return View(adminDetail);
        }

        // POST: AdminDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminDetail = await _context.AdminDetails.FindAsync(id);
            _context.AdminDetails.Remove(adminDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminDetailExists(int id)
        {
            return _context.AdminDetails.Any(e => e.PkAdminId == id);
        }
    }
}
