using DemoFRW.AF;
using DemoFRW.Contrats.FRW;
using DemoFRW.Contrats;
using DemoFRW.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DemoFRW.API.Controllers
{
    [ApiController]
    [Route("api/Formulaires")]
    [Produces("application/json")]
    public class FormulaireController : ControllerBase
    {
        private readonly ILogger<FRWController> _logger;
        private ApiFRW _frw = new ApiFRW();

        public FormulaireController(ILogger<FRWController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtenir les formulaire d'un individu
        /// </summary>
        /// <param name="identifiantUtilisateur"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, object>), StatusCodes.Status200OK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IEnumerable<RetourObtenirFormulairesIndividu>> Get(uint identifiantUtilisateur)
        {
            var retour = await _frw.ObtenirFormulairesIndividu(identifiantUtilisateur);

            return retour.Sortie;
        }

        /// <summary>
        /// Créer un formulaire pour un individu avec un type de formulaire
        /// </summary>
        /// <param name="identifiantUtilisateur">Test2</param>
        /// <param name="typeFormulaire">Test</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Dictionary<string, object>), StatusCodes.Status200OK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<AppelSortant<RetourCreerReprendreFormulaireIndividu>> Post([FromQuery] uint identifiantUtilisateur, [FromBody] string typeFormulaire)
        {
            return await _frw.CreerFormulaireIndividu(typeFormulaire, identifiantUtilisateur);
        }

        /// <summary>
        /// Supprimer un formulaire d'un individu
        /// </summary>
        /// <param name="identifiantUtilisateur"></param>
        /// <param name="noFormulairePublic"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(Dictionary<string, object>), StatusCodes.Status200OK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<AppelSortant> Delete(uint identifiantUtilisateur, string noFormulairePublic)
        {
            return await _frw.SupprimerFormulaire(noFormulairePublic, identifiantUtilisateur, null);
        }
    }
}