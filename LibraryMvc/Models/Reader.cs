using System;
using System.Collections.Generic;

namespace LibraryMvc.Models;

public partial class Reader
{
    public int ReaderId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
