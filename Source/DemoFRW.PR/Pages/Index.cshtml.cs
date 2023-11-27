using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;

namespace DemoFRW.PR.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        IConfiguration _config;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public List<Formulaire> Formulaires { get; set; }

        public string UrlAPI => _config.GetValue<string>("FRW:UrlAPI");

        public string UrlFRW => _config.GetValue<string>("FRW:UrlFRW");

        public uint? IdentifiantUtilisateur => _config.GetValue<uint>("FRW:IdentifiantUtilisateur");

        public string TypeFormulaire => _config.GetValue<string>("FRW:TypeFormulaire");

        public async Task OnGet()
        {
            Formulaires = await ObtenirFormulaires();
        }

        public async Task<PartialViewResult> OnGetTableauFormulaires()
        {
            return Partial("_TableauFormulaires", await ObtenirFormulaires());
        }

        private async Task<List<Formulaire>> ObtenirFormulaires()
        {
            List<Formulaire>? formulaires = new List<Formulaire>();

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(UrlAPI)
            };

            var retour = await httpClient.GetAsync("/api/Formulaires?identifiantUtilisateur=" + IdentifiantUtilisateur);

            if (retour.IsSuccessStatusCode)
            {
                formulaires = await retour.Content.ReadFromJsonAsync<List<Formulaire>>();

                if(formulaires is not null)
                {
                    formulaires.ForEach(f => f.TitreLangueActuelle = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en" ? f.TitreFrancais : f.TitreAnglais);             
                }
                else
                {
                    formulaires = new List<Formulaire>();
                }
            }
            else
            {
                throw new Exception("L'obtention des formulaire a eu un problème.");
            }

            return formulaires;
        }
    }
}