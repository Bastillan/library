using System.ComponentModel.DataAnnotations;

namespace ReactLibrary.Server.Models
{
    public class PostBookDTO
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? Genre { get; set; }
        [Required]
        public string? Publisher { get; set; }
        [Required]
        [Display(Name = "Publication date")]
        [DataType(DataType.Date)]
        public DateTime? PublicationDate { get; set; }
    }
}
