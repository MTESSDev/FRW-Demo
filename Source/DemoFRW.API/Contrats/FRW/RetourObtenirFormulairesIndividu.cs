﻿using System.Runtime.Serialization;

namespace DemoFRW.Contrats.FRW
{
    [DataContract]
    public class RetourObtenirFormulairesIndividu
    {
        [DataMember]
        public string TypeFormulaire { get; set; }

        [DataMember]
        public string NoPublicForm { get; set; }

        [DataMember]
        public string? IdentifiantUtilisateur { get; set; }

        [DataMember]
        public string? TitreFrancais { get; set; }

        [DataMember]
        public string? TitreAnglais { get; set; }

        [DataMember]
        public DateTime DateCreation { get; set; }

        [DataMember]
        public string? DernierEtat { get; set; }

        [DataMember]
        public DateTime DateDernierEtat { get; set; }

        [DataMember]
        public DateTime DateEpuration { get; set; }
    }
}
