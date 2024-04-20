using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Core.ViewModel
{
    public class BookCopyFormViewModel
    {
        public int Id { get; set; }
        [Remote("AllowItem", null!, AdditionalFields = "Id,EditionNumber", ErrorMessage = Errors.Duplicated)]
        public int BookId { get; set; }
        [Display(Name = "Is available for rental")]
        public bool IsAvailableForRental { get; set; }
        [Required]
        [Display(Name = "Edition Number")]
        [Range(1,1000,ErrorMessage =Errors.EditionNumberRange)]
        [Remote("AllowItem", null!, AdditionalFields = "Id,BookId", ErrorMessage = Errors.Duplicated)]
        public int EditionNumber { get; set; }
        public bool ShowRentalInput { get; set; }
    }
}
