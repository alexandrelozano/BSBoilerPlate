using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BSBoilerPlate.Pages
{
    [Route("/[controller]")]
    [ApiController]
    public class Culture : ControllerBase
    {
        public ActionResult SetCulture(string culture, string redirectUri)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture, culture)));

            return LocalRedirect(redirectUri);
        }
    }
}