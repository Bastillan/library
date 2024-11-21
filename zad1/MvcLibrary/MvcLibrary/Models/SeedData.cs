using Microsoft.EntityFrameworkCore;
using MvcLibrary.Data;

namespace MvcLibrary.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Book.Any())
                {
                    return;
                }
                context.Book.AddRange(
                    new Book
                    {
                        Title = "The Great Gatsby",
                        Author = "F. Scott Fitzgerald",
                        Genre = "Classic",
                        Publisher = "Scribner",
                        PublicationDate = DateTime.Parse("1925-04-10"),
                        Status="Available"
                    },
                    new Book
                    {
                        Title = "To Kill a Mockingbird",
                        Author = "Harper Lee",
                        Genre = "Fiction",
                        Publisher = "J.B. Lippincott & Co.",
                        PublicationDate = DateTime.Parse("1960-07-11"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "1984",
                        Author = "George Orwell",
                        Genre = "Dystopian",
                        Publisher = "Secker & Warburg",
                        PublicationDate = DateTime.Parse("1949-06-08"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "Pride and Prejudice",
                        Author = "Jane Austen",
                        Genre = "Romance",
                        Publisher = "T. Egerton",
                        PublicationDate = DateTime.Parse("1813-01-28"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Catcher in the Rye",
                        Author = "J.D. Salinger",
                        Genre = "Fiction",
                        Publisher = "Little, Brown and Company",
                        PublicationDate = DateTime.Parse("1951-07-16"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Hobbit",
                        Author = "J.R.R. Tolkien",
                        Genre = "Fantasy",
                        Publisher = "George Allen & Unwin",
                        PublicationDate = DateTime.Parse("1937-09-21"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "Moby-Dick",
                        Author = "Herman Melville",
                        Genre = "Adventure",
                        Publisher = "Harper & Brothers",
                        PublicationDate = DateTime.Parse("1851-10-18"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "War and Peace",
                        Author = "Leo Tolstoy",
                        Genre = "Historical",
                        Publisher = "The Russian Messenger",
                        PublicationDate = DateTime.Parse("1869-01-01"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Alchemist",
                        Author = "Paulo Coelho",
                        Genre = "Philosophical",
                        Publisher = "HarperOne",
                        PublicationDate = DateTime.Parse("1988-01-01"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "Brave New World",
                        Author = "Aldous Huxley",
                        Genre = "Science Fiction",
                        Publisher = "Chatto & Windus",
                        PublicationDate = DateTime.Parse("1932-08-31"),
                        Status = "Available"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
