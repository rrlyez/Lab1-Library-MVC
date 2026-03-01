using System;
using System.Collections.Generic;

namespace LibraryMvc.Models;

public partial class Issue
{
    public int IssueId { get; set; }

    public int BookId { get; set; }

    public int ReaderId { get; set; }

    public DateOnly IssueDate { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Reader Reader { get; set; } = null!;
}
