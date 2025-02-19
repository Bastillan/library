using System.ComponentModel.DataAnnotations;

namespace ReactLibrary.Server.Models.Api
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        [Display(Name = "Username")]
        public string? UserName { get; set; }
        public int BookId { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        [Display(Name = "Reservation started")]
        [DataType(DataType.Date)]
        public DateTime? ReservationDate { get; set; }
        [Display(Name = "Valid to")]
        [DataType(DataType.Date)]
        public DateTime? ValidDate { get; set; }
    }
}
