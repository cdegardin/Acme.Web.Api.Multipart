namespace Sample.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using System.Web;

    [DataContract]
    public class Registration
    {
        [DataMember(Name = "check")]
        public bool Check { get; set; }

        [DataMember(Name = "contracts")]
        public Collection<HttpPostedFileBase> Contracts { get; } = new Collection<HttpPostedFileBase>();

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "item")]
        public SubItem Item { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "photo")]
        [Required]
        public HttpPostedFileBase Photo { get; set; }
    }

    [DataContract]
    public class SubItem
    {
        [DataMember(Name = "file")]
        public HttpPostedFileBase File { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }
    }
}