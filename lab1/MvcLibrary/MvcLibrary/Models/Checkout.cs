using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models
{
    public class Checkout
    {
        public int Id { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? StartTime {  get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndTime { get; set; }
    }
}
