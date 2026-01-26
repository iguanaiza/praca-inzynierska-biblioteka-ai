using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Models.Relations;

namespace BibliotekaSzkolnaAI.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }

        // Zestawy DbSet reprezentujące tabele w bazie danych
        public DbSet<Book> Books { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<BookSpecialTag> BookSpecialTags { get; set; }
        public DbSet<BookPublisher> BookPublishers { get; set; }
        public DbSet<BookSeries> BookSeries { get; set; }
        public DbSet<BookType> BookTypes { get; set; }
        public DbSet<BookBookGenre> BooksBookGenres { get; set; }
        public DbSet<BookBookSpecialTag> BooksBookSpecialTags { get; set; }
        public DbSet<BookLoan> BookLoans { get; set; }
        public DbSet<BookReservationCart> BookReservationCarts { get; set; }
        public DbSet<FavoriteBook> FavoriteBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // filtr soft delete
            modelBuilder.Entity<Book>().HasQueryFilter(b => !b.IsDeleted);
            modelBuilder.Entity<BookCopy>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);

            // relacja wiele-do-wielu: Book <-> BookGenre
            modelBuilder.Entity<BookBookGenre>()
                .HasKey(bb => new { bb.BookId, bb.BookGenreId });

            modelBuilder.Entity<BookBookGenre>()
                .HasOne(bb => bb.Book)
                .WithMany(b => b.BookBookGenres)
                .HasForeignKey(bb => bb.BookId);

            modelBuilder.Entity<BookBookGenre>()
                .HasOne(bb => bb.BookGenre)
                .WithMany(bg => bg.BookBookGenres)
                .HasForeignKey(bb => bb.BookGenreId);

            // relacja wiele-do-wielu: Book <-> BookSpecialTag
            modelBuilder.Entity<BookBookSpecialTag>()
                .HasKey(bb => new { bb.BookId, bb.BookSpecialTagId });

            modelBuilder.Entity<BookBookSpecialTag>()
                .HasOne(bb => bb.Book)
                .WithMany(b => b.BookBookSpecialTags)
                .HasForeignKey(bb => bb.BookId);

            modelBuilder.Entity<BookBookSpecialTag>()
                .HasOne(bb => bb.BookSpecialTag)
                .WithMany(bg => bg.BookBookSpecialTags)
                .HasForeignKey(bb => bb.BookSpecialTagId);

            // ulubione ksiazki usera
            modelBuilder.Entity<FavoriteBook>()
                .HasKey(fb => new { fb.UserId, fb.BookId });

            modelBuilder.Entity<FavoriteBook>()
                .HasOne(fb => fb.User)
                .WithMany(u => u.FavoriteBooks)
                .HasForeignKey(fb => fb.UserId);

            modelBuilder.Entity<FavoriteBook>()
                .HasOne(fb => fb.Book)
                .WithMany(b => b.FavoriteByUsers)
                .HasForeignKey(fb => fb.BookId);

            // koszyk
            modelBuilder.Entity<BookReservationCart>()
                .HasOne(rc => rc.User)
                .WithMany(u => u.ReservationCart)
                .HasForeignKey(rc => rc.UserId);

            modelBuilder.Entity<BookReservationCart>()
                .HasOne(rc => rc.BookCopy)
                .WithMany(bc => bc.ReservationCart)
                .HasForeignKey(rc => rc.BookCopyId);

            // book loan
            modelBuilder.Entity<BookLoan>()
                .HasOne(bl => bl.User)
                .WithMany(u => u.BookLoans)
                .HasForeignKey(bl => bl.UserId);

            modelBuilder.Entity<BookLoan>()
                .HasOne(bl => bl.BookCopy)
                .WithMany(bc => bc.BookLoans)
                .HasForeignKey(bl => bl.BookCopyId);
        }
    }
}
