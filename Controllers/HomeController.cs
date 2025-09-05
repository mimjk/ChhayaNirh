using System.Web.Mvc;

namespace ChhayaNirh.Controllers
{
    public class HomeController : Controller
    {
        // 🏠 Home Page
        public ActionResult Index()
        {
            return View();
        }

        // ℹ️ About Page
        public ActionResult About()
        {
            ViewBag.Message = "About ChhayaNirh-ছায়ানীড়.";
            return View();
        }

        // 📞 Contact Page
        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us.";
            return View();
        }

    }
}
