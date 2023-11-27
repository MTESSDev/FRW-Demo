using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DemoFRW.Contrats
{
    [DataContract]
    public class RetourCreerReprendreFormulaireIndividu
    {
        [DataMember]
        [JsonProperty("noPublicSession")]
        public string NoPublicSession { get; set; }

        [DataMember]
        [JsonProperty("noPublicForm")]
        public string NoPublicForm { get; set; }

    }
}
