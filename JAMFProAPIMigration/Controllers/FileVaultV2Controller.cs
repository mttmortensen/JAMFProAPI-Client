using JAMFProAPIMigration.Services.Core;
using Microsoft.AspNetCore.Mvc;

namespace JAMFProAPIMigration.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class FileVaultV2Controller : ControllerBase
    {
        private readonly ApiManager _manager;
        public FileVaultV2Controller(ApiManager manager)
        {
            _manager = manager;
        }
    }
}
