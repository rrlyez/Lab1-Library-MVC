using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryMvc.Models;

public partial class Genre
{
    public int GenreId { get; set; }

    [Required(ErrorMessage = "Enter the genre name.")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
