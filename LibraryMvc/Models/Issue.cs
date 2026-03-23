using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryMvc.Models;

public partial class Issue
{
    public int IssueId { get; set; }

    [Required(ErrorMessage = "Select the book.")]
    public int? BookId { get; set; }

    [Required(ErrorMessage = "Select the reader.")]
    public int? ReaderId { get; set; }

    [Required(ErrorMessage = "Select issue date.")]
    public DateTime? IssueDate { get; set; }

    [Required(ErrorMessage = "Select due date.")]
    public DateTime? DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Reader Reader { get; set; } = null!;
}
