using ApiSegundaPracticaMMT.Models;
using System.Security.Claims;

namespace ApiSegundaPracticaMMT.Helpers
{
    public class HelperUsuarioToken
    {
        private IHttpContextAccessor context;
        private HelperCrytography helper;
        private IConfiguration conf;

        public HelperUsuarioToken(IHttpContextAccessor context, HelperCrytography helper, IConfiguration conf)
        {
            this.context = context;
            this.helper = helper;
            this.conf = conf;
        }

        public UsuariosCubo GetUsuario()
        {
            Claim claim = context.HttpContext.User.FindFirst(z => z.Type == "UserData");
            string json = claim.Value;
            string jsonEmpleado = helper.Decrypt(json, conf.GetValue<string>("KeyCryt"));
            UsuariosCubo model = JsonConvert.DeserializeObject<EmpleadoModel>(jsonEmpleado);
            return model;
        }
    }
}
