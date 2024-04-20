using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModel;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Security.Claims;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class BooksCopyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BooksCopyController(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            Book book = _context.Books.SingleOrDefault(x => x.Id == (int)TempData.Peek("BookId"));

            if (book is null)
                return BadRequest();

            BookCopyFormViewModel viewModel = new BookCopyFormViewModel
            {
                BookId = book.Id,
                ShowRentalInput = book.IsAvailableForRental
            };
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookCopyFormViewModel model)
        {
            //model.BookId = (int)TempData.Peek("BookId");
            if (!ModelState.IsValid)
                return BadRequest();

            var book = _context.Books.Find(model.BookId);

            if (book is null)
                return NotFound();

            var copy = _mapper.Map<BookCopy>(model);
            copy.IsAvailableForRental = book.IsAvailableForRental && model.IsAvailableForRental;
            copy.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.BookCopy.Add(copy);
            _context.SaveChanges();
            var viewModel = _mapper.Map<BookCopyViewModel>(copy);
            return PartialView("~/Views/Books/_CopyRow.cshtml", viewModel);

        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int Id)
        {
            var copy = _context.BookCopy.Include(b=>b.Book).SingleOrDefault(c=>c.Id == Id);
            if (copy is null)
                return NotFound();

            var viewModel = _mapper.Map<BookCopyFormViewModel>(copy);
            viewModel.ShowRentalInput = copy.Book!.IsAvailableForRental;
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookCopyFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var copy = _context.BookCopy.Include(c => c.Book).SingleOrDefault(c => c.Id == model.Id);
            if (copy is null)
                return BadRequest();
            copy = _mapper.Map(model, copy);
            copy.IsAvailableForRental = copy.Book!.IsAvailableForRental && model.IsAvailableForRental;
            copy.LastUpdatedOn = DateTime.Now;
            copy.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Update(copy);
            _context.SaveChanges();
            
            var viewModel = _mapper.Map<BookCopyViewModel>(copy);
            return PartialView("~/Views/Books/_CopyRow.cshtml", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var copy = _context.BookCopy.Find(id);
            if (copy is null)
                return NotFound();
            copy.IsDeleted = !copy.IsDeleted;
            copy.LastUpdatedOn = DateTime.Now;
            copy.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();
            return Ok(copy.LastUpdatedOn.ToString());
        }
        public IActionResult AllowItem(BookCopyFormViewModel model)
        {
            var copy = _context.BookCopy.SingleOrDefault(b => b.EditionNumber == model.EditionNumber && b.BookId == model.BookId);
            var isAllow = copy is null || copy.Id.Equals(model.Id);
            return Json(isAllow);
        }
       
    }
}
