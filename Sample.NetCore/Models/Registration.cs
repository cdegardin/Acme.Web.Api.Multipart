namespace Sample.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using Microsoft.AspNetCore.Http;

    [DataContract]
    public class Registration
    {
        [DataMember(Name = "check")]
        public bool Check { get; set; }

        [DataMember(Name = "contracts")]
        public Collection<IFormFile> Contracts { get; } = new Collection<IFormFile>();

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "item")]
        public SubItem Item { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "photo")]
        [Required]
        public IFormFile Photo { get; set; }
    }

    [DataContract]
    public class SubItem
    {
        [DataMember(Name = "file")]
        public IFormFile File { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }
    }
}