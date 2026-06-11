using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
       
        protected IActionResult ValidateRequest(object entity)
        {
            if (entity == null)
            {
                return BadRequest("Os dados enviados não podem estar vazios.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return null; 
        }
    }
}