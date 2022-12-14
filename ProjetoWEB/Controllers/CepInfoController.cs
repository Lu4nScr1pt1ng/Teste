using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoWEB.Data;
using ProjetoWEB.Models;

namespace ProjetoWEB.Controllers
{
    [Route("v1/cep")]
    public class CepInfoController : Controller
    {
        [HttpGet]
        public async Task<ActionResult<List<Cep>>> GetAllCeps(
            [FromServices] DataContext context
            )
        {
            var ceps = await context.Cep.ToListAsync();
            return Ok(ceps);
        }
    }
}
