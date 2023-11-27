using DemoFRW.AF;
using DemoFRW.Contrats;
using DemoFRW.Contrats.FRW;
using Microsoft.AspNetCore.Mvc;

namespace DemoFRW.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class FRWController : ControllerBase
    {

        private readonly ILogger<FRWController> _logger;
        private ApiFRW _frw = new ApiFRW();

        public FRWController(ILogger<FRWController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Démarrer la démo FRW : initialiser le préremplissage, créer un nouveau formulaire, obtenir tout les formulaires d'un utilisateur,
        /// obtenir l'identifiant de session du formulaire crée, supprimer ce formulaire.
        /// </summary>
        /// <returns>Un dictionnaire contenant les détails du démarrage.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, object>), StatusCodes.Status200OK)]
        public async Task<Dictionary<string, object>> DemarrerFRW() {
            return await _frw.DemarrerFRW();
        }
    }
}