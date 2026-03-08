using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryMvc.Models
{
    public class BookFormVM
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Введіть назву книги.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Введіть ISBN.")]
        public string? Isbn { get; set; }

        [Required(ErrorMessage = "Вкажіть рік видання.")]
        [Range(1000, 2026, ErrorMessage = "Рік видання має бути в межах від 1000 до 2026.")]
        public int? PublishYear { get; set; }

        [Required(ErrorMessage = "Оберіть видавця.")]
        [Range(1, int.MaxValue, ErrorMessage = "Оберіть видавця.")]
        public int PublisherId { get; set; }

        [Required(ErrorMessage = "Вкажіть кількість примірників.")]
        [Range(1, 100000, ErrorMessage = "Кількість примірників має бути більшою за 0.")]
        public int CopiesCount { get; set; }

        public List<int> AuthorIds { get; set; } = new();

        public List<int> GenreIds { get; set; } = new();
    }
}