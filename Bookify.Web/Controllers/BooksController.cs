using Bookify.Web.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;
using System.Security.Claims;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Cloudinary _cloudinary;
        private readonly IImageService _imageService;

        private List<string> _allowExtention = new() { ".jpg", ".jpeg", ".png" };
        private int _allowImageSize = 2097152;
        public BooksController(IMapper mapper, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IOptions<CloudinarySetting> cloudinary, IImageService imageService)
        {
            _mapper = mapper;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            Account account = new()
            {
                Cloud = cloudinary.Value.Cloud,
                ApiKey = cloudinary.Value.ApiKey,
                ApiSecret = cloudinary.Value.ApiSecret,
            };
            _cloudinary = new Cloudinary(account);
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetBooks()
        {
            IQueryable<Book> book = _context.Books;
            var skip = int.Parse(Request.Form["start"]);
            var take = int.Parse(Request.Form["length"]);
            var columnSortingIndex = Request.Form["order[0][column]"];
            var columnSortingName = Request.Form[$"columns[{columnSortingIndex}][name]"];
            var searchVal = Request.Form["search[value]"];

            //search
            if(!string.IsNullOrEmpty(searchVal))
            {
                book = book.Where(b => b.Title.Contains(searchVal) || b.Author!.Name.Contains(searchVal));
            }

            //order
            var sortingType = Request.Form["order[0][dir]"];
            book = book.OrderBy($"{columnSortingName} {sortingType}");

            //Take and skabe
            var data = book
                        .Include(b => b.Author)
                        .Include(c => c.Categories)
                        .ThenInclude(c => c.Category)
                        .Skip(skip)
                        .Take(take)
                        .ToList();
            var recordsTotal = book.Count();
            
            //ViewModel
            var bookVM = _mapper.Map<IEnumerable<BookViewModel>>(data);

            //Must by Name and Order
            var json = new { recordsFilterd = recordsTotal, recordsTotal, data = bookVM };
            return Ok(json);
        }
        public IActionResult Details(int Id)
        {
            TempData["BookId"] = Id;
            var book = _context.Books
                               .Include(b=>b.Author)
                               .Include(c => c.Copies)
                               .Include(c=>c.Categories)
                               .ThenInclude(c=>c.Category)
                               .SingleOrDefault(b=>b.Id == Id);
            if(book is null)
                return NotFound();
            var bookVM = _mapper.Map<BookViewModel>(book);
            return View(bookVM);
        }
        public IActionResult Create()
        {
            return View("Form", PrebareBookFormViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", PrebareBookFormViewModel(model));
            }
            var book = _mapper.Map<Book>(model);
            if (model.Image is not null)
            {

                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, "/images/books", hasThumbnail: true);

                if (!isUploaded)
                {
                    ModelState.AddModelError(nameof(Image), errorMessage!);
                    return View("Form", PrebareBookFormViewModel(model));

                }

                book.ImageUrl = $"/images/books/{imageName}";
                book.ThumbnailImageUrl = $"/images/books/thumb/{imageName}";



                //var ImageExtetion = Path.GetExtension(model.Image.FileName);
                //if (!_allowExtention.Contains(ImageExtetion))
                //{
                //    ModelState.AddModelError(nameof(model.Image), Errors.AllowedExtension);
                //    return View("Form", PrebareBookFormViewModel(model));
                //}
                //if (model.Image.Length > _allowImageSize)
                //{
                //    ModelState.AddModelError(nameof(model.Image), Errors.MaxLength);
                //    return View("Form", PrebareBookFormViewModel(model));
                //}
                ////Add Image To Project File
                //var ImageName = $"{Guid.NewGuid()}_{model.Image.Name}{ImageExtetion}";
                //var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/books", ImageName);
                //using var stream = System.IO.File.Create(path);
                //await model.Image.CopyToAsync(stream);
                //book.ImageUrl = $"/Images/books/{ImageName}";
                //stream.Dispose();

                ////ADD Thumbinal
                //var Thumbinalpath = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/books/thumb", ImageName);
                //using var Thumbinalstream = Image.Load(model.Image.OpenReadStream());
                //var width = (float)Thumbinalstream.Width / 200;
                //var height = width / Thumbinalstream.Height;
                //Thumbinalstream.Mutate(i => i.Resize(width: 200, height: (int)height));
                //Thumbinalstream.Save(Thumbinalpath);
                //Thumbinalstream.Dispose();
                //book.ThumbnailImageUrl = $"/Images/books/thumb/{ImageName}";

                //Add Image To Cloud
                /*var ImageName = $"{Guid.NewGuid()}_{model.Image.Name}{ImageExtetion}";
                var stream = model.Image.OpenReadStream();
                var imageParams = new ImageUploadParams
                {
                    File = new FileDescription(ImageName, stream),
                    UseFilename = true
                };
                var result = await _cloudinary.UploadAsync(imageParams);
                book.ImageUrl = result.SecureUrl.ToString();
                book.ThumbnailImageUrl = GetThumbnail(result.SecureUrl.ToString());
                book.PublicIdImage = result.PublicId;
                */
            }
            foreach (var category in model.SelectCategories)
            {
                book.Categories.Add(new BookCategory { CategoryId = category });
            }
            book.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new {Id = book.Id});
        }
        public IActionResult Edit(int Id)
        {
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == Id);
            if (book == null)
                return NotFound();
            var viewModel = _mapper.Map<BookFormViewModel>(book);
            viewModel = PrebareBookFormViewModel(viewModel);
            viewModel.SelectCategories = book.Categories.Select(c => c.CategoryId).ToList();
            viewModel.PublishingDate = book.PublishingDate;
            return View("Form", PrebareBookFormViewModel(viewModel));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Form", PrebareBookFormViewModel(model));
            }
            var book = _context.Books.Include(b=>b.Categories).Include(c=>c.Copies).SingleOrDefault(b => b.Id == model.Id);
            if (book is null)
            {
                return NotFound();
            }
            //string PublicId = null;
            if (model.Image is not null)
            {
                
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    _imageService.Delete(book.ImageUrl, book.ThumbnailImageUrl);

                    //await _cloudinary.DeleteResourcesAsync(book.ImagePublicId);
                }

                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, "/images/books", hasThumbnail: true);

                if (!isUploaded)
                {
                    ModelState.AddModelError(nameof(Image), errorMessage!);
                    return View("Form", PrebareBookFormViewModel(model));
                }

                model.ImageUrl = $"/images/books/{imageName}";
                model.ThumbnailImageUrl = $"/images/books/thumb/{imageName}";


                    //    // Image In project File

                    //    var oldImage = $"{_webHostEnvironment.WebRootPath}{book.ImageUrl}";
                    //    if (System.IO.File.Exists(oldImage))
                    //    {
                    //        System.IO.File.Delete(oldImage);
                    //    }
                    //    var oldThumbImage = $"{_webHostEnvironment.WebRootPath}{book.ThumbnailImageUrl}";
                    //    if (System.IO.File.Exists(oldThumbImage))
                    //    {
                    //        System.IO.File.Delete(oldThumbImage);
                    //    }

                    //    //Image In Cloud
                    //    //await _cloudinary.DeleteResourcesAsync(book.PublicIdImage);
                    //}
                    //var ImageExtetion = Path.GetExtension(model.Image.FileName);
                    //if (!_allowExtention.Contains(ImageExtetion))
                    //{
                    //    ModelState.AddModelError(nameof(model.Image), Errors.AllowedExtension);
                    //    return View("Form", PrebareBookFormViewModel(model));
                    //}
                    //if (model.Image.Length > _allowImageSize)
                    //{
                    //    ModelState.AddModelError(nameof(model.Image), Errors.MaxLength);
                    //    return View("Form", PrebareBookFormViewModel(model));
                    //}

                    //// Image In project File
                    //var ImageName = $"{Guid.NewGuid()}_{model.Image.Name}{ImageExtetion}";
                    //var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/books", ImageName);
                    //using var stream = System.IO.File.Create(path);
                    //await model.Image.CopyToAsync(stream);
                    //stream.Dispose();
                    //model.ImageUrl = $"/Images/books/{ImageName}";
                    ////Thumb Image
                    //var Thumbinalpath = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/books/thumb", ImageName);
                    //using var Thumbinalstream = Image.Load(model.Image.OpenReadStream());
                    //var width = (float)Thumbinalstream.Width / 200;
                    //var height = width / Thumbinalstream.Height;
                    //Thumbinalstream.Mutate(i => i.Resize(width: 200, height: (int)height));
                    //Thumbinalstream.Save(Thumbinalpath);
                    //Thumbinalstream.Dispose();
                    //model.ThumbnailImageUrl = $"/Images/books/thumb/{ImageName}";

                    //Add Image To Cloud
                    /*var ImageName = $"{Guid.NewGuid()}_{model.Image.Name}{ImageExtetion}";
                    var stream = model.Image.OpenReadStream();
                    var imageParams = new ImageUploadParams
                    {
                        File = new FileDescription(ImageName, stream),
                        UseFilename = true
                    };
                    var result = await _cloudinary.UploadAsync(imageParams);
                    PublicId = result.PublicId;
                    model.ImageUrl = result.SecureUrl.ToString();
                    */
            }
            else if(!string.IsNullOrEmpty(book.ImageUrl))
            {
                model.ImageUrl = book.ImageUrl;
                if (!string.IsNullOrEmpty(book.ThumbnailImageUrl))
                {
                    model.ThumbnailImageUrl = book.ThumbnailImageUrl;
                }
            }
            book = _mapper.Map(model, book);
            book.LastUpdatedOn = DateTime.Now;
            book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            //Image in Cloud
            //book.ThumbnailImageUrl = GetThumbnail(book.ImageUrl!);
            //book.PublicIdImage = PublicId;

            foreach (var category in model.SelectCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });
            
            if(!model.IsAvailableForRental)
            foreach (var copy in book.Copies)
            {
                copy.IsAvailableForRental = false;
            }
            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { Id = book.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var book = _context.Books.Find(id);
            if (book is null)
                return NotFound();
            book.IsDeleted = !book.IsDeleted;
            book.LastUpdatedOn = DateTime.Now;
            book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();
            return Ok();
        }
        private BookFormViewModel PrebareBookFormViewModel(BookFormViewModel? model = null)
        {
            BookFormViewModel viewModel = model ?? new BookFormViewModel();
            var auther = _context.Authors.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();
            var catergory = _context.Categories.Where(c => !c.IsDeleted).OrderBy(a => a.Name).ToList();

            viewModel.Author = _mapper.Map<IEnumerable<SelectListItem>>(auther);
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(catergory);
            return viewModel;
        }
        public IActionResult AllowItem(BookFormViewModel model)
        {
            var book = _context.Books.SingleOrDefault(b => b.Title == model.Title && b.AuthorId == model.AuthorId);
            var isAllow = book is null || book.Id.Equals(model.Id);
            return Json(isAllow);
        }
        private string GetThumbnail(string Url)
        {
            //https://res.cloudinary.com/dyaa/image/upload/v1707072442/einoyypxznojsfcj6u8t.jpg
            //https://res.cloudinary.com/dyaa/image/upload/c_thumb,w_200,g_face/v1707072442/einoyypxznojsfcj6u8t.jpg
            
            var seperate = "image/upload/";
            var split = Url.Split(seperate);
            var Thumbnail = $"{split[0]}{seperate}c_thumb,w_200,g_face/{split[1]}";
            return Thumbnail;
        }
    }
}