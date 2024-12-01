using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models
{
    public class CheckoutBookViewModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public int BookId { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartTime { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndTime { get; set; }
    }
}
