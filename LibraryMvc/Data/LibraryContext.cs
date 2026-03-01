using System;
using System.Collections.Generic;
using LibraryMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryMvc.Data;

public partial class LibraryContext : DbContext
{
    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Issue> Issues { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=library_db;Username=postgres;Password=Nosova1002");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("author_pkey");

            entity.ToTable("author");

            entity.Property(e => e.AuthorId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("author_id");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasColumnName("last_name");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("book_pkey");

            entity.ToTable("book");

            entity.HasIndex(e => e.Isbn, "uq_book_isbn")
                .IsUnique()
                .HasFilter("(isbn IS NOT NULL)");

            entity.Property(e => e.BookId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("book_id");
            entity.Property(e => e.CopiesCount)
                .HasDefaultValue(0)
                .HasColumnName("copies_count");
            entity.Property(e => e.Isbn).HasColumnName("isbn");
            entity.Property(e => e.PublishYear).HasColumnName("publish_year");
            entity.Property(e => e.PublisherId).HasColumnName("publisher_id");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_book_publisher");

            entity.HasMany(d => d.Authors).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookAuthor",
                    r => r.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_book_author_author"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("fk_book_author_book"),
                    j =>
                    {
                        j.HasKey("BookId", "AuthorId").HasName("book_author_pkey");
                        j.ToTable("book_author");
                        j.IndexerProperty<int>("BookId").HasColumnName("book_id");
                        j.IndexerProperty<int>("AuthorId").HasColumnName("author_id");
                    });

            entity.HasMany(d => d.Genres).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_book_genre_genre"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("fk_book_genre_book"),
                    j =>
                    {
                        j.HasKey("BookId", "GenreId").HasName("book_genre_pkey");
                        j.ToTable("book_genre");
                        j.IndexerProperty<int>("BookId").HasColumnName("book_id");
                        j.IndexerProperty<int>("GenreId").HasColumnName("genre_id");
                    });
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("genre_pkey");

            entity.ToTable("genre");

            entity.HasIndex(e => e.Name, "uq_genre_name").IsUnique();

            entity.Property(e => e.GenreId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("genre_id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Issue>(entity =>
        {
            entity.HasKey(e => e.IssueId).HasName("issue_pkey");

            entity.ToTable("issue");

            entity.Property(e => e.IssueId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("issue_id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.IssueDate).HasColumnName("issue_date");
            entity.Property(e => e.ReaderId).HasColumnName("reader_id");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");

            entity.HasOne(d => d.Book).WithMany(p => p.Issues)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_issue_book");

            entity.HasOne(d => d.Reader).WithMany(p => p.Issues)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_issue_reader");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("publisher_pkey");

            entity.ToTable("publisher");

            entity.Property(e => e.PublisherId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("publisher_id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(e => e.ReaderId).HasName("reader_pkey");

            entity.ToTable("reader");

            entity.Property(e => e.ReaderId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("reader_id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.Phone).HasColumnName("phone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
