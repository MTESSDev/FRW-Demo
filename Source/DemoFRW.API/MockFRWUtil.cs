using DemoFRW.Contrats.FRW;
using CSharpVitamins;
using DemoFRW.Contrats;

namespace DemoFRW.API
{
	public class MockFRWUtil
	{

		public MockFRWUtil() { }

		/// <summary>
		/// Genere un faux RetourCreerReprendreFormulaireIndividu pour l'application de démo
		/// </summary>
		/// <returns></returns>
		public static RetourCreerReprendreFormulaireIndividu ObtenirRetourCreerReprendreFormulaireIndividu() {
			return new RetourCreerReprendreFormulaireIndividu()
			{
				NoPublicForm = ShortGuid.NewGuid().ToString(),
				NoPublicSession = ShortGuid.NewGuid().ToString()
			};
		}

		/// <summary>
		/// Retourn une fausse liste de RetourObtenirFormulaireIndividu pour la démo
		/// </summary>
		/// <param name="identifiantUtilisateur"></param>
		/// <returns></returns>
		public static IEnumerable<RetourObtenirFormulairesIndividu> InitialiserListeRetourObtenirFormulaireIndividu(uint identifiantUtilisateur) {

			List<RetourObtenirFormulairesIndividu> listeRetour = new List<RetourObtenirFormulairesIndividu>();

			//On genere des faux RetourObtenirFormulairesIndividu
			for (int i = 0; i < 5; i++) {
				listeRetour.Add(ObtenirRetourObtenirFormulaireIndividu(identifiantUtilisateur));
			}

			return listeRetour;
		}

		/// <summary>
		/// Genere un faux RetourObtenirFormulairesIndividu pour l'application de démo
		/// </summary>
		/// <param name="identifiantUtilisateur"></param>
		/// <returns></returns>
		public static RetourObtenirFormulairesIndividu ObtenirRetourObtenirFormulaireIndividu(uint identifiantUtilisateur) {

			Random random = new Random();

			string idUtilisateur = identifiantUtilisateur.ToString();
			string typeForm = random.Next(3000,3006).ToString();

			string titreFr = $"Formulaire {typeForm}";
			string titreEn = $"Form {typeForm}";
			string noPublicForm = ShortGuid.NewGuid().ToString();
			DateTime dateActuelle = DateTime.Now;

			return new RetourObtenirFormulairesIndividu()
			{
				TypeFormulaire = typeForm,
				NoPublicForm = noPublicForm,
				IdentifiantUtilisateur = idUtilisateur,
				TitreFrancais = titreFr,
				TitreAnglais = titreEn,
				DateCreation = dateActuelle,
				DateDernierEtat = dateActuelle,
				DateEpuration = dateActuelle.AddDays(3)
			};
		}

	}
}
