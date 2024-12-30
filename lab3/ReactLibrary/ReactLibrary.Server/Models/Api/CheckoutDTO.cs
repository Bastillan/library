using System.ComponentModel.DataAnnotations;

namespace ReactLibrary.Server.Models.Api
{
    public class CheckoutDTO
    {
        public int Id { get; set; }
        [Display(Name = "Username")]
        public string? UserName { get; set; }
        public int BookId { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        [Display(Name = "Start time")]
        [DataType(DataType.Date)]
        public DateTime? StartTime { get; set; }
        [Display(Name = "End time")]
        [DataType(DataType.Date)]
        public DateTime? EndTime { get; set; }
    }
}
