using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MvcLibrary.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string? UserName { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        [Display(Name = "Reservation started")]
        [DataType(DataType.Date)]
        public DateTime? ReservationDate { get; set; }
        [Required]
        [Display(Name = "Valid to")]
        [DataType(DataType.Date)]
        public DateTime? ValidDate { get; set; }
    }
}
