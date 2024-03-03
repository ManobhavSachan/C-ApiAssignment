using System.ComponentModel.DataAnnotations;

namespace Try_pls.Models
{
    public interface Entity1
    {

        List<Address> Addresses { get; set; }
        List<Date> Dates { get; set; }
        bool Deceased { get; set; }
        string Gender { get; set; }
        List<Name> Names { get; set; }
    }

    public class UpdateContactRequest : Entity1
    {

        public List<Address> Addresses { get; set; }
        public List<Date> Dates { get; set; }
        public bool Deceased { get; set; }
        // [Required(AllowEmptyStrings = true)]
        public string Gender { get; set; }
        public List<Name> Names { get; set; }
        
    }
}
