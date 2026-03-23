using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryMvc.Models;

public partial class Publisher
{
    public int PublisherId { get; set; }

    [Required(ErrorMessage = "Enter the name of the publisher.")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
