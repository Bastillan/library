using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models
{
    public class Checkout
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string? UserName { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        [Display(Name = "Start time")]
        [DataType(DataType.Date)]
        public DateTime? StartTime {  get; set; }
        [Display(Name = "End time")]
        [DataType(DataType.Date)]
        public DateTime? EndTime { get; set; }
    }
}
