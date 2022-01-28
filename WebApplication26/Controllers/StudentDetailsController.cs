﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using WebApplication26.Models;
using System.Collections;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication26.Controllers
{
    public class StudentDetailsController : Controller
    {
        private readonly project1211Context _context;
        private readonly IWebHostEnvironment hostEnvironment;
        public StudentDetailsController(project1211Context context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;

        }

        // GET: StudentDetails
        [Authorize(Roles = "Student,Faculty,Admin")]
        public async Task<IActionResult> Index()
        {
            var CurrentUserIDSession = HttpContext.Session.GetString("name");
            if (string.IsNullOrEmpty(CurrentUserIDSession))
            {

                return RedirectToAction("Index", "Login");

            }
            else
            {
                return View(await _context.StudentDetails.ToListAsync());
            }


        }

        // GET: StudentDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentDetail = await _context.StudentDetails
                .FirstOrDefaultAsync(m => m.PkStudentId == id);
            if (studentDetail == null)
            {
                return NotFound();
            }

            return View(studentDetail);
        }
       [Authorize(Roles = "Student")]
        public async Task<IActionResult> Myprofilee(int? id)
        {var Email = HttpContext.Session.GetString("Email");
            var CurrentUserIDSession = HttpContext.Session.GetString("name");
            if (string.IsNullOrEmpty(CurrentUserIDSession))
            {

                return RedirectToAction("Index", "Login");

            }
            else
            {
                if (Email == null)
                {
                    return NotFound();
                }
                var studentDetail = await _context.StudentDetails
                 .FirstOrDefaultAsync(m => m.Email == Email);
                id = studentDetail.PkStudentId;

                ViewBag.attendance = _context.AttendanceDetails.Count(a => a.FkStudId == id);

                ViewBag.attended = _context.AttendanceDetails.Count(a => a.FkStudId == id && a.Attendance == "Present");
                if (ViewBag.attendance == 0)
                {
                    ViewBag.attendance = "Not updated";
                    ViewBag.attended = "Not updated";
                }
                
                var list = _context.MarksDetails.Where(a => a.FkStudId == id).OrderBy(a => a.FkSemId);
                var count = _context.MarksDetails.Count(a => a.FkStudId == id);
                ArrayList l1 = new ArrayList();
                for (int i = 1; i < 9; i++)
                {
                    int check = 0;
                    if (count != 0)
                    {
                        foreach (var a in list)
                        {
                            if (a.FkSemId == i)
                            {
                                l1.Add(Math.Round((float)(((a.SessionalMarks + a.MainExamMarks) * 100) / a.TotalMarks), 2));
                                check = 1;
                            }

                        }
                        if (check == 0) { l1.Add("Pending"); }
                    }
                    else {

                        if (check == 0) { l1.Add("Pending"); }
                    }
                  

                }

                ViewBag.marks = l1;

                if (studentDetail == null)
                {
                    return NotFound();
                }

                return View(studentDetail);
            }
        }
        // GET: StudentDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StudentDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PkStudentId,EnrollId,Email,FirstName,LastName,DateOfBirth,Contact,Address,Pswd,Course,FatherName,CreatedDate,IsActive,IsDeleted,")] StudentDetail studentDetail)
        { 
          
            if (ModelState.IsValid)
            {
                var ac = Convert.ToDateTime(studentDetail.DateOfBirth);

                DateTime PresentYear = DateTime.Now;
                TimeSpan ts = PresentYear - ac;
                if (ts.Days > 0)
                {
                    DateTime Age = DateTime.MinValue.AddDays(ts.Days);
                    if (Age.Year - 1 < 18)
                    {
                        ModelState.AddModelError("", "Age cant be less than 18");
                        return View(studentDetail);
                    }
                }
                else 
                {
                    ModelState.AddModelError("", "Dob cant be future date");
                    return View(studentDetail);
                }

                if (studentDetail.EnrollId == null || studentDetail.Email == null || studentDetail.Pswd == null || studentDetail.FirstName == null) { return View(studentDetail); }
                var record_Check = _context.MstUsers.FirstOrDefault(m => m.Email == studentDetail.Email || m.Contact == studentDetail.Contact);
                if (record_Check != null)
                { ModelState.AddModelError("", "Email or Mobile Assosiated with other Account"); return View(studentDetail); }
                studentDetail.CreatedDate = DateTime.Now;
                _context.Add(studentDetail);
                await _context.SaveChangesAsync();
                var CurrentUserIDSession = HttpContext.Session.GetString("name");

                if (string.IsNullOrEmpty(CurrentUserIDSession))
                {

                    return RedirectToAction("Index", "Login");

                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(studentDetail);
        }


        // GET: StudentDetails/Edit/5
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null||id==0)
            {
                var getid = (from s in _context.StudentDetails
                             where s.Email == HttpContext.Session.GetString("Email")
                             select s.PkStudentId).SingleOrDefault();
                id = getid;
            }

            if (id == null)
            {
                return NotFound();
            }

            var studentDetail = await _context.StudentDetails.FindAsync(id);
            if (studentDetail == null)
            {
                return NotFound();
            }
            return View(studentDetail);
        }

        // POST: StudentDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PkStudentId,EnrollId,Email,FirstName,LastName,DateOfBirth,Contact,Address,Pswd,Course,FatherName,CreatedDate,IsActive,IsDeleted,PProfilepic,studentPic")] StudentDetail studentDetail)
        {
            if (id == 0||id==null) {
                var getid = (from s in _context.StudentDetails
                             where s.Email == HttpContext.Session.GetString("Email")
                             select s.PkStudentId).SingleOrDefault();
                id = getid;
            }
            
            
            if (id != studentDetail.PkStudentId)
            {
                return NotFound();
            }
           if (studentDetail.PProfilepic!=null)
            {
                string wwwRootPath = hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(studentDetail.PProfilepic.FileName);
                string ext = Path.GetExtension(studentDetail.PProfilepic.FileName);
                studentDetail.studentPic = fileName + DateTime.Now.ToString("yymmssff") + ext;
                string path = Path.Combine(wwwRootPath + "/images/", studentDetail.studentPic);
                using (var filestrem = new FileStream(path, FileMode.Create))
                {
                    await studentDetail.PProfilepic.CopyToAsync(filestrem);
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentDetailExists(studentDetail.PkStudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (HttpContext.Session.GetString("Type") == "1") { return RedirectToAction("Myprofilee", "StudentDetails"); }
                else { return RedirectToAction(nameof(Index)); }
              
            }
            if (HttpContext.Session.GetString("Type") == "1") { return RedirectToAction("Myprofilee", "StudentDetails"); }
            else { return RedirectToAction(nameof(Index)); }
        }

        // GET: StudentDetails/Delete/5
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentDetail = await _context.StudentDetails
                .FirstOrDefaultAsync(m => m.PkStudentId == id);
            if (studentDetail == null)
            {
                return NotFound();
            }

            return View(studentDetail);
        }

        // POST: StudentDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentDetail = await _context.StudentDetails.FindAsync(id);
            _context.StudentDetails.Remove(studentDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentDetailExists(int id)
        {
            return _context.StudentDetails.Any(e => e.PkStudentId == id);
        }
    }
}