using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryMvc.Models;

public partial class Reader
{
    public int ReaderId { get; set; }

    [Required(ErrorMessage = "Enter the reader's full name.")]
    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
