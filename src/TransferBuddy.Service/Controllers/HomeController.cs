using Microsoft.AspNetCore.Mvc;

namespace ECB.Service.Controllers
{
    /// <summary>
    /// The home controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// The index endpoint.
        /// </summary>
        /// <returns>A view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// The error endpoint.
        /// </summary>
        /// <returns>A view.</returns>
        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// The configure endpoint.
        /// </summary>
        /// <returns>A view.</returns>
        public IActionResult Configure()
        {
            return View();
        }
    }
}
