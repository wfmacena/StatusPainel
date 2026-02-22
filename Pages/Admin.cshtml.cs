using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StatusPainel.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ILogger<AdminModel> _logger;

        public AdminModel(ILogger<AdminModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}