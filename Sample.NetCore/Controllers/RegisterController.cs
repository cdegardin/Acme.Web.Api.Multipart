using System.Web.Http;

namespace Sample.Controllers
{
    public class RegisterController : ApiController
    {
        [HttpPost]
        public Registration Post(Registration registration)
        {
            return registration;
        }
    }
}