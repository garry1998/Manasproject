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
    public class FacultyDetailsController : Controller
    {
        private readonly project1211Context _context;

        public FacultyDetailsController(project1211Context context)
        {
            _context = context;
        }

        // GET: FacultyDetails
        public async Task<IActionResult> Index()
        {
            var CurrentUserIDSession = HttpContext.Session.GetString("name");
            if (string.IsNullOrEmpty(CurrentUserIDSession))
            {

                return RedirectToAction("Index", "Login");

            }
            else
            {
                var project1211Context = _context.FacultyDetails.Include(f => f.FkDept);
                return View(await project1211Context.ToListAsync());
            }
        }

        // GET: FacultyDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facultyDetail = await _context.FacultyDetails
                .Include(f => f.FkDept)
                .FirstOrDefaultAsync(m => m.PkFacultyId == id);
            if (facultyDetail == null)
            {
                return NotFound();
            }

            return View(facultyDetail);
        }

        // GET: FacultyDetails/Create
        public IActionResult Create()
        {
            ViewData["FkDeptId"] = new SelectList(_context.MstDepartments, "PkDeptId", "DepartmentName");
            return View();
        }

        // POST: FacultyDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PkFacultyId,FacultyId,FirstName,LastName,DateOfBirth,Address,Email,Contact,Pswd,FkDeptId,FatherName,CreatedDate,IsActive,IsDeleted")] FacultyDetail facultyDetail)
        {
            if (ModelState.IsValid)
            {
                var CurrentUserIDSession = HttpContext.Session.GetString("name");
                var ac = Convert.ToDateTime(facultyDetail.DateOfBirth);
                
                DateTime PresentYear = DateTime.Now;
                TimeSpan ts = PresentYear - ac;
                if (ts.Days > 0)
                {
                    DateTime Age = DateTime.MinValue.AddDays(ts.Days);
                    if (Age.Year - 1 < 18)
                    {
                        ModelState.AddModelError("", "Age cant be less than 18");
                        return View(facultyDetail);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dob cant be future date");
                    return View(facultyDetail);
                }

                if (facultyDetail.FacultyId == null||facultyDetail.Email==null||facultyDetail.Pswd==null||facultyDetail.FirstName==null) { ModelState.AddModelError("", "Field can't be left blank"); return View(facultyDetail); }
                var record_Check =  _context.MstUsers.FirstOrDefault(m => m.Email == facultyDetail.Email || m.Contact==facultyDetail.Contact);
                if (record_Check!=null)
                { ModelState.AddModelError("", "Email or Mobile Assosiated with other Account"); return View(facultyDetail); }
                _context.Add(facultyDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkDeptId"] = new SelectList(_context.MstDepartments, "PkDeptId", "PkDeptId", facultyDetail.FkDeptId);
            return View(facultyDetail);
        }

        // GET: FacultyDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facultyDetail = await _context.FacultyDetails.FindAsync(id);
            if (facultyDetail == null)
            {
                return NotFound();
            }
            ViewData["FkDeptId"] = new SelectList(_context.MstDepartments, "PkDeptId", "PkDeptId", facultyDetail.FkDeptId);
            return View(facultyDetail);
        }

        // POST: FacultyDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PkFacultyId,FacultyId,FirstName,LastName,DateOfBirth,Address,Email,Contact,Pswd,FkDeptId,FatherName,CreatedDate,IsActive,IsDeleted")] FacultyDetail facultyDetail)
        {
            if (id != facultyDetail.PkFacultyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facultyDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyDetailExists(facultyDetail.PkFacultyId))
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
            ViewData["FkDeptId"] = new SelectList(_context.MstDepartments, "PkDeptId", "PkDeptId", facultyDetail.FkDeptId);
            return View(facultyDetail);
        }

        // GET: FacultyDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facultyDetail = await _context.FacultyDetails
                .Include(f => f.FkDept)
                .FirstOrDefaultAsync(m => m.PkFacultyId == id);
            if (facultyDetail == null)
            {
                return NotFound();
            }

            return View(facultyDetail);
        }

        // POST: FacultyDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facultyDetail = await _context.FacultyDetails.FindAsync(id);
            _context.FacultyDetails.Remove(facultyDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacultyDetailExists(int id)
        {
            return _context.FacultyDetails.Any(e => e.PkFacultyId == id);
        }
    }
}
