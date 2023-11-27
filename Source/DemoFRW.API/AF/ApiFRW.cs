using DemoFRW.Contrats;
using DemoFRW.Contrats.FRW;
using Newtonsoft.Json;
using System.Text;
using YamlDotNet.Serialization.NamingConventions;
using YamlHttpClient;
using YamlHttpClient.Utils;
using YamlDotNet.Serialization;
using DemoFRW.API;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DemoFRW.AF
{
    public class ApiFRW
    {

        private readonly string urlFrw;
        private readonly string apiKey;
        private readonly string noPublicSystemeAutorise;

        private readonly IConfiguration _config;
        private IDeserializer _deserializer;

        //Mocks de la demo
        private bool demo;
        private List<RetourObtenirFormulairesIndividu> listeFormulaires;

        public ApiFRW() {
            _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            _deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            //On va chercher les constantes FRW dans la config
            apiKey = _config["FRW:CleApi"];
            noPublicSystemeAutorise = _config["FRW:NumeroPublicSystemeAutorise"];
            urlFrw = _config["FRW:UrlFRW"];

            //On initialise les mocks. Le boolean demo sert a arreter les call a FRW, vous pouvez le passer a false si vous avez mis une cle d'API et un numéro public de systeme autorise dans le appsettings.json
            demo = true;
            if(demo) initialiserListeForm();
        }

        /// <summary>
        /// SWAGGER :
        /// Exemple d'opérations à réaliser lorsque l'on souhaite appeler FRW.
        /// Pour cet exemple, nous utiliseront un formulaire de type 3003 pour l'utilisateur 1332.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> DemarrerFRW()
        {
            //Paramètres d'exemple dans la config, à adapter selon votre situation
            string typeFormulaire = _config.GetValue<string>("FRW:TypeFormulaire");
            uint idUtilisateur = _config.GetValue<uint>("FRW:IdentifiantUtilisateur");

            //Ici, nous avons un dictionnaire avec les données de préremplissage provenant de votre application, que vous possèdez avant le traitement.
            Dictionary<string, object?> contexteApplication = await ObtenirContexteApplication();

            //Ce dictionnaire est uniquement présent afin d'afficher, dans le SWAGGER, les résultats de toutes les opérations réalisées en exemple.
            Dictionary<string, object> sortieDemarrerFRW = new Dictionary<string, object>();


            //La premiere etape est d'obtenir le JSON de pre remplissage provenant de notre système que l'on souhaite passer dans le formulaire FRW à partir de notre dictionnaire contexteApplication
            string preremplissage = await PreRemplissageFormulaire(typeFormulaire, contexteApplication);


            //Une fois le json de preremplissage formaté, nous avons accès à plusieurs méthodes de FRW! En voici quelques unes :
            sortieDemarrerFRW["CreerFormulaireIndividu"] = await CreerFormulaireIndividu(typeFormulaire, idUtilisateur, preremplissage);

            var retourObtenirFormulairesIndividus = await ObtenirFormulairesIndividu(idUtilisateur);
            sortieDemarrerFRW["ObtenirFormulairesIndividu"] = retourObtenirFormulairesIndividus;
            
            sortieDemarrerFRW["ObtenirIdentifiantSessionFormulaire"] = await ObtenirIdentifiantSessionFormulaire(retourObtenirFormulairesIndividus.Sortie.ToArray()[retourObtenirFormulairesIndividus.Sortie.ToArray().Length - 1].NoPublicForm, idUtilisateur, contexteApplication);

            sortieDemarrerFRW["SupprimerFormulaire"] = await SupprimerFormulaire(retourObtenirFormulairesIndividus.Sortie.ToArray()[retourObtenirFormulairesIndividus.Sortie.ToArray().Length - 1].NoPublicForm, idUtilisateur, contexteApplication);

            return sortieDemarrerFRW;
        }

        /// <summary>
        /// Retourne la liste des formulaire en cours pour l'utilisateur actuel.
        /// </summary>
        /// <param name="identifiantUtilisateur">Identifiant de l'utilisateur dont l'on souhaite obtenir ses formulaires</param>
        /// <returns></returns>
        public async Task<AppelSortant<IEnumerable<RetourObtenirFormulairesIndividu>>> ObtenirFormulairesIndividu(uint identifiantUtilisateur)
        {
            AppelSortant<IEnumerable<RetourObtenirFormulairesIndividu>> retour;


			if (demo)
            {
                //Pour la demo, nous mockons un retour, utilisez directement la methode AppelerFRW dans votre projet.
                retour = new AppelSortant<IEnumerable<RetourObtenirFormulairesIndividu>>() { Sortie = listeFormulaires, Erreurs = new List<Erreur>() };
            }
            else {
				retour = await AppelerFRW<IEnumerable<RetourObtenirFormulairesIndividu>>(
						   HttpMethod.Post,
						   $"ObtenirFormulairesIndividu",
						   identifiantUtilisateur);
			}

            return retour;
        }

        /// <summary>
        /// Obtient l'identifiant session d'un formulaire à partir de son numéro public.
        /// </summary>
        /// <param name="noFormulairePublic">Le numéro public du formulaire</param>
        /// <param name="identifiantUtilisateur">Identifiant de l'utilisateur lié au numéro public du formulaire</param>
        /// <returns></returns>
        public async Task<AppelSortant<RetourCreerReprendreFormulaireIndividu>> ObtenirIdentifiantSessionFormulaire(string noFormulairePublic, uint identifiantUtilisateur, Dictionary<string, object?>? contexteApplication, string? preRemplissage = "{}")
        {
            AppelSortant<RetourCreerReprendreFormulaireIndividu> retour;

            if (demo)
            {
				//Pour la demo, nous mockons un retour, utilisez directement la methode AppelerFRW dans votre projet.
				retour = new AppelSortant<RetourCreerReprendreFormulaireIndividu>()
				{
					Sortie = MockFRWUtil.ObtenirRetourCreerReprendreFormulaireIndividu(),
					Erreurs = new List<Erreur>()
				};
			}
            else { 
                retour = await AppelerFRW<RetourCreerReprendreFormulaireIndividu>(
                    HttpMethod.Post,
                    $"ObtenirIdentifiantSessionFormulaire/{noFormulairePublic}",
                    identifiantUtilisateur, preRemplissage);
            }

            return retour;
        }

        /// <summary>
        /// Crée un nouveau formulaire du type passé en entré.
        /// </summary>
        /// <param name="typeFormulaire">Le type de formulaire à créer.</param>
        /// <param name="identifiantUtilisateur">Identifiant de l'utilisateur qui veut créer le formulaire.</param>
        /// <returns></returns>
        public async Task<AppelSortant<RetourCreerReprendreFormulaireIndividu>> CreerFormulaireIndividu(string typeFormulaire, uint identifiantUtilisateur, string? preremplissage = "{}")
        {
            AppelSortant<RetourCreerReprendreFormulaireIndividu> retour;

            if (demo)
            {
                //Pour la demo, nous mockons un retour, utilisez directement la methode AppelerFRW dans votre projet.
                retour = new AppelSortant<RetourCreerReprendreFormulaireIndividu>()
                {
                    Sortie = MockFRWUtil.ObtenirRetourCreerReprendreFormulaireIndividu(),
                    Erreurs = new List<Erreur>()
                };

                //On ajoute ce form a la liste temporaire /!\ UNIQUEMENT UTILE AU MOCK /!\
                listeFormulaires.Add(new RetourObtenirFormulairesIndividu()
                {
                    TypeFormulaire = typeFormulaire,
                    NoPublicForm = retour.Sortie.NoPublicForm,
                    IdentifiantUtilisateur = identifiantUtilisateur.ToString(),
                    TitreFrancais = "Nouveau formulaire",
                    TitreAnglais = "New form",
                    DateCreation = DateTime.Now,
                    DateDernierEtat = DateTime.Now,
                    DateEpuration = DateTime.Now.AddDays(1)
                });
                enregistrerListeForm();
            }
            else {
				retour = await AppelerFRW<RetourCreerReprendreFormulaireIndividu>(
					HttpMethod.Post,
					$"CreerFormulaireIndividu/{typeFormulaire}",
					identifiantUtilisateur, preremplissage);
			}

            return retour;
        }

        /// <summary>
        /// Effectue un appel à FRW pour supprimer un formulaire.
        /// </summary>
        /// <param name="noFormulairePublic">Le numéro public du formulaire à supprimer.</param>
        /// <param name="identifiantUtilisateur">L'identifiant de l'utilisateur correspondant au numéro public du formulaire à supprimer.</param>
        /// <returns></returns>
        public async Task<AppelSortant> SupprimerFormulaire(string noFormulairePublic, uint identifiantUtilisateur, Dictionary<string, object> contexteApplication)
        {
            AppelSortant retour;

			string preremplissage = JsonConvert.SerializeObject(new
            {
                contexteApplication
            });


            if (demo)
            {
                //Pour la demo, nous mockons un retour, utilisez directement la methode AppelerFRW dans votre projet.
                listeFormulaires = listeFormulaires.Where(x => x.NoPublicForm != noFormulairePublic).ToList();
                retour = new AppelSortant() { Erreurs = new List<Erreur>() };
                enregistrerListeForm();
			}
            else {
				retour = await AppelerFRW<Object>(
				HttpMethod.Get,
				$"SupprimerFormulaire/{noFormulairePublic}",
				identifiantUtilisateur, preremplissage);
			}

            return retour;
        }

        /// <summary>
        /// Télécharger un document de FRW.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Lance une exception si on ne peut pas obtenir les documents</remarks>
        public async Task<AppelSortant<byte[]>> TelechargerDocument(string nomEncode, string preRemplissage)
        {
            var retour = await AppelerFRWReadBytes<byte[]>(
                HttpMethod.Get,
                $"Telecharger/{nomEncode}",preRemplissage);

            return retour;
        }

        /// <summary>
        /// Préremplissage du formulaire pour FRW
        /// </summary>
        private async Task<string> PreRemplissageFormulaire(string typeFormulaire, Dictionary<string, object?> dictionnaireObjet)
        {

            dictionnaireObjet ??= new Dictionary<string, object?>();
            string? contenuJson = "{}";

            // Cherchez ici vos données que vous souhaitez passer dans le dictionnaire de préremplissage
            dictionnaireObjet.TryAdd("Courriel", "courrielDeTest@test.gouv.qc.ca");
            dictionnaireObjet.TryAdd("CodePermanent", "GKOw929fj@JFh9");

            // On vient ensuite génerer notre string de préremplissage JSON en allant appliquer HandleBars sur notre dictionnaire
            // Notons qu'il faudrait aller rechercher votre propre string de template HandleBars, préférablement située dans votre document de configuration de la même manière que dans l'exemple
            string? templateHandleBars = obtenirTemplateExempleHandleBars();

            if (templateHandleBars is not null && dictionnaireObjet is not null)
            {
                contenuJson = new ContentHandler(YamlHttpClientFactory
                    .CreateEmptyHandleBars()
                    .AddBase64()
                    .AddIfCond(true)
                    .AddJsonHelper(new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd" }))
                    .ParseContent(templateHandleBars!, dictionnaireObjet);
            }

            return contenuJson;
        }

        /// <summary>
        /// Permet d'obtenir une string de template HandleBars depuis le fichier exemple.config.yml
        /// </summary>
        /// <returns></returns>
        private string? obtenirTemplateExempleHandleBars()
        {
            // On va chercher le champ preRemplissage dans la config. Notez que nous n'avons pas là un document config.yml complet, étant donné que nous n'avons conservé que ce qui était utile à l'exemple
            var configExemple = _deserializer.Deserialize<Dictionary<string,string>>(File.ReadAllText("Resources\\exemple.config.yml"));
            
            return configExemple["preRemplissage"];
        }

        /// <summary>
        /// Permet d'obtenir les données de préremplissage provenant de votre application
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<string, object?>> ObtenirContexteApplication()
        {
            var contexteApplication = new Dictionary<string, object?>();

            //Ici vos méthodes permettant de remplir le dictionnaire de préremplissage, nous passons un objet bidon pour l'exemple
            string nomUtilisateur = ObtenirNomUtilisateur();

            //Puis ici, on ajoute notre objet de contexte au dictionnaire de données de préremplissage
            contexteApplication["contexteApp"] = new
            {
                NomUtilisateur = nomUtilisateur,
                Age = 42,
                Acces = "utilisateur"
            };

            contexteApplication["contexteApp"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(contexteApplication["contexteApp"]).ToString()));

            return contexteApplication;
        }

        /// <summary>
        /// Methode pour le mock
        /// </summary>
        private void initialiserListeForm() {

			if (!Directory.Exists("Resources"))
			{
				Directory.CreateDirectory("Resources");
			}
			if (!File.Exists("Resources\\MockListeForm"))
			{
				listeFormulaires = new List<RetourObtenirFormulairesIndividu>();
				listeFormulaires = MockFRWUtil.InitialiserListeRetourObtenirFormulaireIndividu(_config.GetValue<uint>("FRW:IdentifiantUtilisateur")).ToList();
                enregistrerListeForm();
			}
			else
			{
                try
                {
                    listeFormulaires = JArray.Parse(File.ReadAllText("Resources\\MockListeForm")).ToObject<List<RetourObtenirFormulairesIndividu>>();
                }
                catch (Exception e) {
					listeFormulaires = MockFRWUtil.InitialiserListeRetourObtenirFormulaireIndividu(_config.GetValue<uint>("FRW:IdentifiantUtilisateur")).ToList();
					enregistrerListeForm();
				}
				
			}
		}

        /// <summary>
        /// Methode pour le mock
        /// </summary>
        private void enregistrerListeForm() {
			File.WriteAllText("Resources\\MockListeForm", JArray.FromObject(listeFormulaires).ToString());
		}

        /// <summary>
        /// Un méthode bidon uniquement présente afin de représenter une recherche
        /// </summary>
        /// <returns></returns>
        private string ObtenirNomUtilisateur()
        {
            return @"mes\norch00";
        }

        /// <summary>
        /// Appèle le SIS du côté FRW pour les opérations en lien avec les formulaires.
        /// </summary>
        /// <typeparam name="T">Le type de retour</typeparam>
        /// <param name="httpMethod">La type de méthode HTTP (GET ou POST)</param>
        /// <param name="api">Le nom de l'opération sur l'API</param>
        /// <param name="jsonData">Les données JSON de l'appel de service.</param>
        /// <returns></returns>
        private async Task<AppelSortant<byte[]>> AppelerFRWReadBytes<T>(HttpMethod httpMethod, string api, string jsonData = "{}")
        {
            var urlServiceDestinationComplet = string.Empty;
            try
            {
                var retour = new byte[0];
                var noPublicSystemeAutorise = this.noPublicSystemeAutorise;
                var apiKey = this.apiKey;

                var urlServiceDestination = urlFrw;

                urlServiceDestinationComplet = $"{urlServiceDestination}/api/v1/Document/{api}";

                using (var request = new HttpRequestMessage(httpMethod, urlServiceDestinationComplet))
                {
                    using var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    if (httpMethod == HttpMethod.Post)
                    {
                        request.Content = stringContent;
                    }

                    request.Headers.Add("X-ApiKey", apiKey);
                    request.Headers.Add("X-NoPublicSystemeAutorise", noPublicSystemeAutorise);
                    request.Headers.Add("Accept", "application/x-msgpack");

                    using (var client = new HttpClient())
                    {
                        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                        {
                            var rep = await response.Content.ReadAsByteArrayAsync();
                            if (response.IsSuccessStatusCode)
                            {
                                retour = rep;
                            }
                            else
                            {
                                throw new Exception($"Erreur {urlServiceDestinationComplet} \r\n {response.StatusCode} - {response.ReasonPhrase}\r\n body : \r\n {response.Content.ReadAsStringAsync().Result}");
                            }
                        }
                    }
                }
                return new AppelSortant<byte[]> { Sortie = retour! };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + $" - Erreur avec l'appel de FRW {urlServiceDestinationComplet}");

                throw;
            }
        }

        /// <summary>
        /// Appèle le SIS du côté FRW pour les opérations en lien avec les formulaires.
        /// </summary>
        /// <typeparam name="T">Le type de retour</typeparam>
        /// <param name="httpMethod">La type de méthode HTTP (GET ou POST)</param>
        /// <param name="api">Le nom de l'opération sur l'API</param>
        /// <param name="identifiantUtilisateur">L'identifiant du dossier courant (passé dans les entêtes)</param>
        /// <param name="jsonData">Les données JSON de l'appel de service.</param>
        /// <returns></returns>
        private async Task<AppelSortant<T>> AppelerFRW<T>(HttpMethod httpMethod, string api, uint identifiantUtilisateur, string jsonData = "{}")
        {

            var urlServiceDestinationComplet = string.Empty;
            try
            {
                T retour = default!;
                var noPublicSystemeAutorise = this.noPublicSystemeAutorise;
                var apiKey = this.apiKey;

                var urlServiceDestination = urlFrw;

                urlServiceDestinationComplet = $"{urlServiceDestination}/api/v1/SIS/{api}";

                using (var request = new HttpRequestMessage(httpMethod, urlServiceDestinationComplet))
                {
                    using var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    if (httpMethod == HttpMethod.Post)
                    {
                        request.Content = stringContent;
                    }

                    request.Headers.Add("X-ApiKey", apiKey);
                    request.Headers.Add("X-NoPublicSystemeAutorise", noPublicSystemeAutorise);
                    request.Headers.Add("X-IdentifiantUtilisateur", identifiantUtilisateur.ToString());
                    using (var client = new HttpClient())
                    {
                        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                        {
                            var rep = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode)
                            {
                                retour = JsonConvert.DeserializeObject<T>(rep)!;
                            }
                            else
                            {
                                throw new Exception($"Erreur {urlServiceDestinationComplet} \r\n {response.StatusCode} - {response.ReasonPhrase}\r\n body : \r\n {response.Content.ReadAsStringAsync().Result}");
                            }
                        }
                    }
                }
                return new AppelSortant<T> { Sortie = retour! };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + $" - Erreur avec l'appel de FRW {urlServiceDestinationComplet}");
                throw;
            }
        }

    }
}
