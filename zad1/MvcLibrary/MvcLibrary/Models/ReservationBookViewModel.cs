﻿using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models
{
    public class ReservationBookViewModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public int BookId { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        [Display(Name = "Reservation started")]
        [DataType(DataType.Date)]
        public DateTime? ReservationDate { get; set; }
        [Required]
        [Display(Name = "Valid to")]
        [DataType(DataType.Date)]
        public DateTime? ValidDate { get; set; }
    }
}
