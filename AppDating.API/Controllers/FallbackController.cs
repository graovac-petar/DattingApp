using Microsoft.AspNetCore.Mvc;

namespace AppDating.API.Controllers
{
    public class FallbackController : Controller
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Index.html"), "text/HTML");
        }
    }
}
