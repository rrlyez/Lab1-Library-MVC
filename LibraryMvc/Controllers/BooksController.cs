using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryMvc.Data;
using LibraryMvc.Models;

namespace LibraryMvc.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.Genres);

            return View(await libraryContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            FillBookSelections();
            return View(new BookFormVM());
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                FillBookSelections(vm.PublisherId, vm.AuthorIds, vm.GenreIds);
                return View(vm);
            }

            var book = new Book
            {
                Title = vm.Title,
                Isbn = vm.Isbn,
                PublishYear = vm.PublishYear,
                PublisherId = vm.PublisherId.Value,
                CopiesCount = vm.CopiesCount
            };

            var authors = await _context.Authors
                .Where(a => vm.AuthorIds.Contains(a.AuthorId))
                .ToListAsync();

            foreach (var author in authors)
            {
                book.Authors.Add(author);
            }

            var genres = await _context.Genres
                .Where(g => vm.GenreIds.Contains(g.GenreId))
                .ToListAsync();

            foreach (var genre in genres)
            {
                book.Genres.Add(genre);
            }

            try
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Failed to create book. Please check that the fields are filled in correctly.");
                FillBookSelections(vm.PublisherId, vm.AuthorIds, vm.GenreIds);
                return View(vm);
            }
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            var vm = new BookFormVM
            {
                BookId = book.BookId,
                Title = book.Title,
                Isbn = book.Isbn,
                PublishYear = book.PublishYear,
                PublisherId = book.PublisherId,
                CopiesCount = book.CopiesCount,
                AuthorIds = book.Authors.Select(a => a.AuthorId).ToList(),
                GenreIds = book.Genres.Select(g => g.GenreId).ToList()
            };

            FillBookSelections(vm.PublisherId, vm.AuthorIds, vm.GenreIds);

            return View(vm);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookFormVM vm)
        {
            if (id != vm.BookId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                FillBookSelections(vm.PublisherId, vm.AuthorIds, vm.GenreIds);
                return View(vm);
            }

            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            book.Title = vm.Title;
            book.Isbn = vm.Isbn;
            book.PublishYear = vm.PublishYear;
            book.PublisherId = vm.PublisherId.Value;
            book.CopiesCount = vm.CopiesCount;

            book.Authors.Clear();
            var authors = await _context.Authors
                .Where(a => vm.AuthorIds.Contains(a.AuthorId))
                .ToListAsync();

            foreach (var author in authors)
            {
                book.Authors.Add(author);
            }

            book.Genres.Clear();
            var genres = await _context.Genres
                .Where(g => vm.GenreIds.Contains(g.GenreId))
                .ToListAsync();

            foreach (var genre in genres)
            {
                book.Genres.Add(genre);
            }

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Failed to save changes.");
                FillBookSelections(vm.PublisherId, vm.AuthorIds, vm.GenreIds);
                return View(vm);
            }
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book != null)
            {
                book.Authors.Clear();
                book.Genres.Clear();
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void FillBookSelections(int? selectedPublisherId = null, List<int>? selectedAuthorIds = null, List<int>? selectedGenreIds = null)
        {
            ViewData["PublisherId"] = new SelectList(
                _context.Publishers,
                "PublisherId",
                "Name",
                selectedPublisherId
            );

            ViewData["Authors"] = new MultiSelectList(
                _context.Authors,
                "AuthorId",
                "LastName",
                selectedAuthorIds
            );

            ViewData["Genres"] = new MultiSelectList(
                _context.Genres,
                "GenreId",
                "Name",
                selectedGenreIds
            );
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}