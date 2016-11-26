using Microsoft.AspNetCore.Mvc;

using TransferBuddy.Service.Controllers
{
    [Route("api/[controller]")]
    public class RateController : Controller
    {
        [HttpGet]
        public List<Rate> Get([FromQuery]string source, [FromQuery]string target, [FromQuery]string type)
        {
            var repository = new RateRepository(source, target, type);
            return repository.Get();
        }
    }
}