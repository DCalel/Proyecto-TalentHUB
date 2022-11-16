using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TalentHUB.Models
{
    public class Register : IComparable
    {
        [Display(Name = "Nombre")]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [Display(Name = "DPI")]
        [JsonProperty(PropertyName = "dpi")]
        public string Dpi { get; set; }

        [Display(Name = "Fecha de nacimiento")]
        [JsonProperty(PropertyName = "datebirth")]
        public string Datebirth { get; set; }

        [Display(Name = "Dirección")]
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [Display(Name = "Compañias")]
        [JsonProperty(PropertyName = "companies")]
        public string[] Companies { get; set; }

        [Display(Name = "Reclutador")]
        [JsonProperty(PropertyName = "recluiter")]
        public string recluiter { get; set; }


        public List<string> ConversationsE { get; set; }
        public List<string> ConversationsD { get; set; }

        [JsonIgnore]
        public Comparison<Register> InsertByDpi = delegate (Register register1, Register register2)
        {
            return register1.Dpi.CompareTo(register2.Dpi);
        };

        [JsonIgnore]
        public Comparison<Register> InsertByName = delegate (Register registro1, Register register2)
        {
            return registro1.Name.CompareTo(register2.Name);
        };

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
