using DemoFRW.AF;
using DemoFRW.Contrats.FRW;
using DemoFRW.Contrats;
using DemoFRW.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DemoFRW.API.Controllers
{
    [ApiController]
    [Route("api/Sessions")]
    [Produces("application/json")]
    public class SessionController : ControllerBase
    {
        private readonly ILogger<FRWController> _logger;
        private ApiFRW _frw = new ApiFRW();

        public SessionController(ILogger<FRWController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Générer et obtenir la session pour un formulaire
        /// </summary>
        /// <param name="identifiantUtilisateur"></param>
        /// <param name="noFormulairePublic"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, object>), StatusCodes.Status200OK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<AppelSortant<RetourCreerReprendreFormulaireIndividu>> Get(uint identifiantUtilisateur, string noFormulairePublic)
        {
            return await _frw.ObtenirIdentifiantSessionFormulaire(noFormulairePublic, identifiantUtilisateur, null);
        }
    }
}