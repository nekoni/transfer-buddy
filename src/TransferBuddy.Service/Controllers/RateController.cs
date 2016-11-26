using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TransferBuddy.Models;
using TransferBuddy.Repositories;

namespace TransferBuddy.Service.Controllers
{
    [Route("api/[controller]")]
    public class RateController : Controller
    {
        [HttpGet]
        public List<Rate> Index([FromQuery]string source, [FromQuery]string target, [FromQuery]string type)
        {
            var repository = new RateRepository(source, target, type);
            return repository.Get().Result.ToList();;
        }
    }
}