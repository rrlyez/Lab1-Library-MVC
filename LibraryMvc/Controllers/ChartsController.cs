using LibraryMvc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly LibraryContext _context;

        public ChartsController(LibraryContext context)
        {
            _context = context;
        }

        private record CountByYearResponseItem(string Year, int Count);
        private record CountByReaderResponseItem(string Reader, int Count);

        [HttpGet("booksByYear")]
        public async Task<JsonResult> GetBooksByYear(CancellationToken cancellationToken)
        {
            var responseItems = await _context.Books
                .Where(b => b.PublishYear.HasValue)
                .GroupBy(b => b.PublishYear!.Value)
                .OrderBy(g => g.Key)
                .Select(g => new CountByYearResponseItem(g.Key.ToString(), g.Count()))
                .ToListAsync(cancellationToken);

            return new JsonResult(responseItems);
        }

        [HttpGet("issuesByReader")]
        public async Task<JsonResult> GetIssuesByReader(CancellationToken cancellationToken)
        {
            var responseItems = await _context.Issues
                .Include(i => i.Reader)
                .GroupBy(i => i.Reader.FullName)
                .OrderBy(g => g.Count())
                .Select(g => new CountByReaderResponseItem(g.Key, g.Count()))
                .ToListAsync(cancellationToken);

            return new JsonResult(responseItems);
        }
    }
}