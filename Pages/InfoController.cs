using Microsoft.AspNetCore.Mvc;

namespace BSBoilerPlate.Pages
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        [Route("ipaddress")]
        public async Task<string> GetIpAddress()
        {
            var remoteIpAddress = this.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            if (remoteIpAddress != null)
                return remoteIpAddress.ToString();
            return string.Empty;
        }
    }
}
