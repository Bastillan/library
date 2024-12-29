using System.ComponentModel.DataAnnotations;

namespace ReactLibrary.Server.Models
{
    public class PutBookDTO
    {
        [Required]
        public int Id { get; set; }
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
        [Required]
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
