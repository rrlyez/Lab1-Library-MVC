using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryMvc.Data;
using LibraryMvc.Models;

namespace LibraryMvc.Controllers
{
    public class IssuesController : Controller
    {
        private readonly LibraryContext _context;

        public IssuesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Issues
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.Issues
                .Include(i => i.Book)
                .Include(i => i.Reader);

            return View(await libraryContext.ToListAsync());
        }

        // GET: Issues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues
                .Include(i => i.Book)
                .Include(i => i.Reader)
                .FirstOrDefaultAsync(m => m.IssueId == id);

            if (issue == null)
            {
                return NotFound();
            }

            return View(issue);
        }

        // GET: Issues/Create
        public IActionResult Create()
        {
            FillIssueSelections();
            return View();
        }

        // POST: Issues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IssueId,BookId,ReaderId,IssueDate,DueDate")] Issue issue)
        {
            ModelState.Remove("Book");
            ModelState.Remove("Reader");

            if (issue.DueDate < issue.IssueDate)
            {
                ModelState.AddModelError("DueDate", "Дата повернення за планом не може бути раніше дати видачі.");
            }

            var book = await _context.Books.FindAsync(issue.BookId);

            if (book == null)
            {
                ModelState.AddModelError("BookId", "Обрану книгу не знайдено.");
            }
            else if (book.CopiesCount <= 0)
            {
                ModelState.AddModelError("BookId", "Немає доступних примірників цієї книги.");
            }

            if (!ModelState.IsValid)
            {
                FillIssueSelections(issue.BookId, issue.ReaderId);
                return View(issue);
            }

            issue.ReturnDate = null;

            issue.IssueDate = DateTime.SpecifyKind(issue.IssueDate, DateTimeKind.Utc);
            issue.DueDate = DateTime.SpecifyKind(issue.DueDate, DateTimeKind.Utc);

            book!.CopiesCount -= 1;

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Issues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            FillIssueSelections(issue.BookId, issue.ReaderId);
            return View(issue);
        }

        // POST: Issues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IssueId,BookId,ReaderId,IssueDate,DueDate,ReturnDate")] Issue issue)
        {
            ModelState.Remove("Book");
            ModelState.Remove("Reader");

            if (id != issue.IssueId)
            {
                return NotFound();
            }

            if (issue.DueDate < issue.IssueDate)
            {
                ModelState.AddModelError("DueDate", "Дата повернення за планом не може бути раніше дати видачі.");
            }

            var existingIssue = await _context.Issues
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.IssueId == id);

            if (existingIssue == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                FillIssueSelections(issue.BookId, issue.ReaderId);
                return View(issue);
            }

            try
            {
                if (existingIssue.ReturnDate == null && issue.ReturnDate != null)
                {
                    var book = await _context.Books.FindAsync(issue.BookId);
                    if (book != null)
                    {
                        book.CopiesCount += 1;
                    }
                }
                issue.IssueDate = DateTime.SpecifyKind(issue.IssueDate, DateTimeKind.Utc);
                issue.DueDate = DateTime.SpecifyKind(issue.DueDate, DateTimeKind.Utc);

                if (issue.ReturnDate.HasValue)
                {
                    issue.ReturnDate = DateTime.SpecifyKind(issue.ReturnDate.Value, DateTimeKind.Utc);
                }
                _context.Update(issue);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(issue.IssueId))
                {
                    return NotFound();
                }
                throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Не вдалося зберегти зміни.");
                FillIssueSelections(issue.BookId, issue.ReaderId);
                return View(issue);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Issues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues
                .Include(i => i.Book)
                .Include(i => i.Reader)
                .FirstOrDefaultAsync(m => m.IssueId == id);

            if (issue == null)
            {
                return NotFound();
            }

            return View(issue);
        }

        // POST: Issues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var issue = await _context.Issues.FindAsync(id);

            if (issue != null)
            {
                if (issue.ReturnDate == null)
                {
                    var book = await _context.Books.FindAsync(issue.BookId);
                    if (book != null)
                    {
                        book.CopiesCount += 1;
                    }
                }

                _context.Issues.Remove(issue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IssueExists(int id)
        {
            return _context.Issues.Any(e => e.IssueId == id);
        }

        private void FillIssueSelections(int? selectedBookId = null, int? selectedReaderId = null)
        {
            ViewData["BookId"] = new SelectList(
                _context.Books
                    .Select(b => new
                    {
                        b.BookId,
                        Display = b.Title + " (доступно: " + b.CopiesCount + ")"
                    })
                    .ToList(),
                "BookId",
                "Display",
                selectedBookId
            );

            ViewData["ReaderId"] = new SelectList(
                _context.Readers
                    .Select(r => new
                    {
                        r.ReaderId,
                        r.FullName
                    })
                    .ToList(),
                "ReaderId",
                "FullName",
                selectedReaderId
            );
        }
    }
}