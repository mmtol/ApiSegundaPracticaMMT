using ApiSegundaPracticaMMT.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiSegundaPracticaMMT.Helpers
{
    public class HelperUsuarioToken
    {
        private IHttpContextAccessor context;
        private HelperCrytography helper;
        private ModelVault vault;

        public HelperUsuarioToken(IHttpContextAccessor context, HelperCrytography helper, ModelVault vault)
        {
            this.context = context;
            this.helper = helper;
            this.vault = vault;
        }

        public UsuariosCubo GetUsuario()
        {
            Claim claim = context.HttpContext.User.FindFirst(z => z.Type == "UserData");
            string json = claim.Value;
            string jsonUsuario = helper.Decrypt(json, vault.CryptExamen);
            UsuariosCubo model = JsonConvert.DeserializeObject<UsuariosCubo>(jsonUsuario);
            return model;
        }
    }
}
