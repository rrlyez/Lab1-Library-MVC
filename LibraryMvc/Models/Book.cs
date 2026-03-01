using System;
using System.Collections.Generic;

namespace LibraryMvc.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string? Isbn { get; set; }

    public int? PublishYear { get; set; }

    public int PublisherId { get; set; }

    public int CopiesCount { get; set; }

    public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();

    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
