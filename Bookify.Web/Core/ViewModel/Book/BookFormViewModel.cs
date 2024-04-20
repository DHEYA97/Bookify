using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModel
{
    public class BookFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(500)]
        [Remote("AllowItem", null, AdditionalFields = "Id,AuthorId", ErrorMessage = Errors.DuplicatedBook)]
        public string Title { get; set; } = null!;
        [Display(Name = "Author")]
        [Remote("AllowItem", null, AdditionalFields = "Id,Title", ErrorMessage = Errors.DuplicatedBook)]
        public int AuthorId { get; set; }   
        public IEnumerable<SelectListItem>? Author { get; set; }

        [MaxLength(200)]
        public string Publisher { get; set; } = null!;
        [Display(Name = "Publishing Date")]
        [AssertThat("PublishingDate <= Today()",ErrorMessage =Errors.NotAllowFutureDates)]
        public DateTime PublishingDate { get; set; } = DateTime.Now;

        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailImageUrl { get; set; }
        [MaxLength(50)]
        public string Hall { get; set; } = null!;
        [Display(Name = "Is available for rental")]
        public bool IsAvailableForRental { get; set; }

        public string Description { get; set; } = null!;
        [Display(Name= "Categories")]
        public IList<int> SelectCategories { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
