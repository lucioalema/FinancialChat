using FinancialChat.Data;
using FinancialChat.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinancialChat.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        public List<string> Users { get; set; }
        public List<Messages> Messages { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public void OnGet()
        {
            Users = _applicationDbContext.Users
                .Where(x => x.UserName != HttpContext.User.Identity.Name)
                .Select(x => x.UserName)
                .ToList();
        }

        public IActionResult OnGetMessages(string user)
        {
            return new JsonResult(_applicationDbContext.Messages
                .Where(x => (x.UserFrom == HttpContext.User.Identity.Name && x.UserTo == user) || 
                            (x.UserFrom == user && x.UserTo == HttpContext.User.Identity.Name))
                .OrderByDescending(x => x.DateTime)
                .Take(50)
                .OrderBy(x => x.DateTime)
                .ToList());
        }
    }
}