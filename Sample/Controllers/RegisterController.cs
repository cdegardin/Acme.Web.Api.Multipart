namespace Sample.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using Sample.ViewModels;

    public class RegisterController: ApiController
    {
        [HttpPost]
        public Registration Post(Registration registration)
        {
            return registration;
        }
    }
}