using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Data;
using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Controllers
{
    public class ClassesController : Controller
    {
        private readonly SchoolMgmtContext _context;

        public ClassesController(SchoolMgmtContext context)
        {
            _context = context;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            var schoolMgmtContext = _context.Classes.Include(c => c.Course).Include(l => l.Lecturer);
            return View(await schoolMgmtContext.ToListAsync());
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(c => c.Course)
                .Include(l => l.Lecturer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // Display user relevant Course and Lecturer data for Class Create, Edit
        private void BuildSelectLists()
        {
            var courses = _context.Courses.Select(q => new
            {
                CourseNameDetails = $"{q.Code} {q.CourseName} ({q.Credits} Credits)",
                q.Id
            });
            ViewData["CourseId"] = new SelectList(courses, "Id", "CourseNameDetails");

            var lecturers = _context.Lecturers.Select(q => new
            {
                FullName = $"{q.LastName}, {q.FirstName}",
                q.Id
            });
            ViewData["LecturerId"] = new SelectList(lecturers, "Id", "FullName");
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            BuildSelectLists();
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LecturerId,CourseId,Time")] Class @class)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // If Post fails, redisplay records
            BuildSelectLists();
            return View(@class);
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }

            BuildSelectLists();
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LecturerId,CourseId,Time")] Class @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
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
            BuildSelectLists();
            return View(@class);
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(c => c.Course)
                .Include(l => l.Lecturer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'SchoolMgmtContext.Classes'  is null.");
            }
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

// TODO: WHY DID changing id to classId cause query to fail????????? or rather id to be 0 ???
        public async Task<IActionResult> ManageEnrollments(int id)
        { 
            var @class = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Lecturer)
                .Include(c => c.Enrollments)
                    .ThenInclude(c => c.Student)  // TODO: revisit this
            .FirstOrDefaultAsync(m => m.Id == id);
            
            var @students = await _context.Students.ToListAsync();

            // Transform to View Model
            var model = new ClassEnrollmentViewModel();

// TODO: Handle errors if class is null
            // Get class info from Data Model (@class) into View Model
            model.Class = new ClassViewModel {
                Id = @class.Id,
                CourseName = $"{@class.Course?.Id} - {@class.Course?.CourseName}",
                LecturerName = $"{@class.Lecturer?.FirstName} {@class.Lecturer?.LastName}",
                CourseTime = @class.Time?.ToString()
            }; 

            // Add Enrollment data to the student list, check if student is enrolled in the class
            foreach (var student in students)
            {
                model.Students.Add(new StudentEnrollmentViewModel(){
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    IsEnrolled = (@class?.Enrollments?.Any(q => q.StudentId == student.Id)).GetValueOrDefault()
                }); // 
            }
            return View(model);
        }

        [HttpPost, ActionName("EnrollStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollStudent(int classId, int studentId, bool shouldEnroll)
        {
            var enrollment = new Enrollment();

            if (shouldEnroll)
            {
                enrollment.ClassId = classId;
                enrollment.StudentId = studentId;
                // Add the enrollment to the Data table; Entity Framework will determine the correct table by the data type
                await _context.AddAsync(enrollment);
            }
            else
            {
                // Get the first matching record from the database. We don't have the id, but we have the Class and Student Id;
                //  there can be only one record on which they match (a student can't be enrolled twice in same class).
                // This time we must specify the Enrollments table 
                enrollment = await _context.Enrollments.FirstOrDefaultAsync(
                    q => q.ClassId == classId && q.StudentId == studentId);
                if (enrollment != null)
                {
                    _context.Remove(enrollment);
                    // Could also have been more explicit:
                    // _context.Enrollments.Remove(enrollment);
                    // But again the table is clear to Entity Framework because of the type passed
                }
            }

            await _context.SaveChangesAsync();
            // 2nd param of redirect is an object whose purpose is only to route values to the action
            return RedirectToAction(nameof(ManageEnrollments), new {id = classId}); 
        }

        private bool ClassExists(int id)
        {
            return (_context.Classes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
