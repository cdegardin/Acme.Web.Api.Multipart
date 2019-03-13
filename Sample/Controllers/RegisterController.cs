namespace Sample.Controllers
{
    using System.Web.Http;

    using Sample.ViewModels;

    public class RegisterController : ApiController
    {
        [HttpPost]
        public Registration Post(Registration registration)
        {
            return registration;
        }
    }
}