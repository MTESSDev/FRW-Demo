using System.Runtime.Serialization;

namespace DemoFRW.Contrats
{
    /// <summary>
    /// Classe générique d'appel sortant
    /// </summary>
    [DataContract]
    public class AppelSortant
    {
        /// <summary>
        /// Liste des erreurs
        /// </summary>
        [DataMember]
        public IEnumerable<Erreur>? Erreurs { get; set; }

        public bool EnErreur => Erreurs != null && Erreurs.Any();
    }

    /// <summary>
    /// Classe générique d'appel sortant
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class AppelSortant<T> : AppelSortant
    {
        /// <summary>
        /// Données de l'appel
        /// </summary>
        [DataMember]
        public T Sortie { get; set; } = default!;

    }
}
