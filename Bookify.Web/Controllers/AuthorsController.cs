namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthorsController(ApplicationDbContext _context, IMapper mapper)
        {
            this._context = _context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var authors = _context.Authors.AsNoTracking().ToList();
            var viewModel = _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
            return View(viewModel);
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var author = _mapper.Map<Author>(model);
            author.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Add(author);
            _context.SaveChanges();
            var viewModel = _mapper.Map<AuthorViewModel>(author);
            return PartialView("_AuthorRow", viewModel);
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int Id)
        {
            var author = _context.Authors.Find(Id);
            if (author is null)
                return BadRequest();
            var viewModel = _mapper.Map<AuthorFormViewModel>(author);
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var author = _context.Authors.Find(model.Id);
            if (author is null)
                return BadRequest();
            author = _mapper.Map(model, author);
            author.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Update(author);
            _context.SaveChanges();
            var viewModel = _mapper.Map<AuthorViewModel>(author);
            return PartialView("_AuthorRow", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var author = _context.Authors.Find(id);
            if (author is null)
                return NotFound();
            author.IsDeleted = !author.IsDeleted;
            author.LastUpdatedOn = DateTime.Now;
            author.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();
            return Ok(author.LastUpdatedOn.ToString());
        }
        public IActionResult AllowItem(AuthorFormViewModel author)
        {
            var cat = _context.Authors.SingleOrDefault(c => c.Name == author.Name);
            var isAllow = cat is null || cat.Id.Equals(author.Id);
            return Json(isAllow);
        }
    }
}
