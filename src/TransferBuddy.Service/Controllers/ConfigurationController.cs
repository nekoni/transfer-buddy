using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransferBuddy.Models;
using TransferBuddy.Repositories;

namespace TransferBuddy.Service.Controllers
{
    [Authorize]
    public class ConfigurationController : Controller
    {
        ConfigurationRepository repository;

        public ConfigurationController(ConfigurationRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// The index endpoint.
        /// </summary>
        /// <returns>A view.</returns>
        public IActionResult Index([FromQueryAttribute] string userId)
        {
            ViewBag.UserId = userId;
            var configs = this.repository.Get().Result;
            return View(configs);
        }

        /// <summary>
        /// The configure endpoint.
        /// </summary>
        /// <returns>A view.</returns>
        [HttpPost]
        public IActionResult Create([Bind("FacebookId, Description, Source, Target, Frequency")]TransferConfig config)
        {
            if (ModelState.IsValid)
            {
                this.repository.Create(config).Wait();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create([FromQueryAttribute] string userId)
        {
            var config = new TransferConfig() { FacebookId = userId };
            return View(config);
        }
    }
}
