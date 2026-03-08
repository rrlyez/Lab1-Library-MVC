using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryMvc.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    [Required(ErrorMessage = "Введіть ім'я автора.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Введіть прізвище автора.")]
    public string LastName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
