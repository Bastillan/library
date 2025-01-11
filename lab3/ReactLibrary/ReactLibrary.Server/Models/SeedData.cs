using Microsoft.EntityFrameworkCore;
using ReactLibrary.Server.Data;

namespace ReactLibrary.Server.Models
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
                        Status = "Available"
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
                    },
                    new Book
                    {
                        Title = "The Road",
                        Author = "Cormac McCarthy",
                        Genre = "Post-Apocalyptic",
                        Publisher = "Alfred A. Knopf",
                        PublicationDate = DateTime.Parse("2006-09-26"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "Crime and Punishment",
                        Author = "Fyodor Dostoevsky",
                        Genre = "Psychological Fiction",
                        Publisher = "The Russian Messenger",
                        PublicationDate = DateTime.Parse("1866-01-01"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Shining",
                        Author = "Stephen King",
                        Genre = "Horror",
                        Publisher = "Doubleday",
                        PublicationDate = DateTime.Parse("1977-01-28"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "Dune",
                        Author = "Frank Herbert",
                        Genre = "Science Fiction",
                        Publisher = "Chilton Books",
                        PublicationDate = DateTime.Parse("1965-08-01"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Kite Runner",
                        Author = "Khaled Hosseini",
                        Genre = "Drama",
                        Publisher = "Riverhead Books",
                        PublicationDate = DateTime.Parse("2003-05-29"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Book Thief",
                        Author = "Markus Zusak",
                        Genre = "Historical Fiction",
                        Publisher = "Picador",
                        PublicationDate = DateTime.Parse("2005-03-14"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "Sapiens: A Brief History of Humankind",
                        Author = "Yuval Noah Harari",
                        Genre = "Non-Fiction",
                        Publisher = "Harvill Secker",
                        PublicationDate = DateTime.Parse("2011-01-01"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "Frankenstein",
                        Author = "Mary Shelley",
                        Genre = "Science Fiction",
                        Publisher = "Lackington, Hughes, Harding, Mavor & Jones",
                        PublicationDate = DateTime.Parse("1818-01-01"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Name of the Wind",
                        Author = "Patrick Rothfuss",
                        Genre = "Fantasy",
                        Publisher = "DAW Books",
                        PublicationDate = DateTime.Parse("2007-03-27"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "Slaughterhouse-Five",
                        Author = "Kurt Vonnegut",
                        Genre = "Science Fiction",
                        Publisher = "Delacorte",
                        PublicationDate = DateTime.Parse("1969-03-31"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Picture of Dorian Gray",
                        Author = "Oscar Wilde",
                        Genre = "Philosophical Fiction",
                        Publisher = "Ward, Lock & Co.",
                        PublicationDate = DateTime.Parse("1890-07-20"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Giver",
                        Author = "Lois Lowry",
                        Genre = "Dystopian",
                        Publisher = "Houghton Mifflin",
                        PublicationDate = DateTime.Parse("1993-04-26"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Fault in Our Stars",
                        Author = "John Green",
                        Genre = "Young Adult",
                        Publisher = "Dutton Books",
                        PublicationDate = DateTime.Parse("2012-01-10"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "A Game of Thrones",
                        Author = "George R.R. Martin",
                        Genre = "Fantasy",
                        Publisher = "Bantam Books",
                        PublicationDate = DateTime.Parse("1996-08-06"),
                        Status = "Available"
                    },
                    new Book
                    {
                        Title = "The Secret History",
                        Author = "Donna Tartt",
                        Genre = "Mystery",
                        Publisher = "Alfred A. Knopf",
                        PublicationDate = DateTime.Parse("1992-09-01"),
                        Status = "Available"
                    }
                );
                context.SaveChanges();
            }
        }
    }

}
