using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace MvcLibrary.Models
{
    public class BookGenreViewModel
    {
        public IPagedList<Book>? Books { get; set; }
        public SelectList? Genres { get; set; }
        public string? BookGenre {  get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
    }
}
