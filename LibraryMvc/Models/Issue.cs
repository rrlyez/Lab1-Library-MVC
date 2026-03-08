using System;
using System.Collections.Generic;

namespace LibraryMvc.Models;

public partial class Issue
{
    public int IssueId { get; set; }

    public int BookId { get; set; }

    public int ReaderId { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Reader Reader { get; set; } = null!;
}
