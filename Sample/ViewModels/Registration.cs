namespace Sample.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using System.Web;

    [DataContract]
    public class Registration
    {
        [DataMember(Name = "check")]
        public bool Check { get; set; }

        public Collection<HttpPostedFileBase> Contracts { get; } = new Collection<HttpPostedFileBase>();

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [Required]
        public HttpPostedFileBase Photo { get; set; }
    }
}