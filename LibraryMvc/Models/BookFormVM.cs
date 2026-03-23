using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryMvc.Models
{
    public class BookFormVM
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Enter the title of the book.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Enter ISBN.")]
        public string? Isbn { get; set; }

        [Required(ErrorMessage = "Enter a valid publish year.")]
        [Range(1000, 2026, ErrorMessage = "The year of publication must be between 1000 and 2026.")]
        public int? PublishYear { get; set; }

        [Required(ErrorMessage = "Select the publisher.")]
        public int? PublisherId { get; set; }

        [Required(ErrorMessage = "Enter the number of copies.")]
        [Range(1, 100000, ErrorMessage = "Copies count must be greater than 0.")]
        public int CopiesCount { get; set; }

        public List<int> AuthorIds { get; set; } = new();

        public List<int> GenreIds { get; set; } = new();
    }
}