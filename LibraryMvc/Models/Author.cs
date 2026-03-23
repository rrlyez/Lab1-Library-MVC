using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryMvc.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    [Required(ErrorMessage = "Enter the author's first name.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Enter the author's last name.")]
    public string LastName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
