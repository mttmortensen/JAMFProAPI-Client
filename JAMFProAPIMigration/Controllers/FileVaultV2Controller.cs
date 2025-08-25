using JAMFProAPIMigration.Interfaces;
using JAMFProAPIMigration.Services.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JAMFProAPIMigration.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class FileVaultV2Controller : ControllerBase
    {
        private readonly IFileVault2 _fv2Service;

        public FileVaultV2Controller(IFileVault2 fv2Service)
        {
            _fv2Service = fv2Service;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllFV2Users()
        {
            var users = await _fv2Service.GetFileVaultInventoryAsync();

            if (users == null)
                return NotFound();

            return Ok(users);
        }
    }
}
