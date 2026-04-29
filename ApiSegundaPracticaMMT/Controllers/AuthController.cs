using ApiSegundaPracticaMMT.Helpers;
using ApiSegundaPracticaMMT.Models;
using ApiSegundaPracticaMMT.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiSegundaPracticaMMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryCubos repo;
        private HelperActionOAuthService helperService;
        private HelperCrytography helperCrypt;
        private ModelVault vault;

        public AuthController(RepositoryCubos repo, HelperActionOAuthService helper, HelperCrytography helperCrypt, ModelVault vault)
        {
            this.repo = repo;
            this.helperService = helper;
            this.helperCrypt = helperCrypt;
            this.vault = vault;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> LogIn(LoginModel model)
        {
            UsuariosCubo user = await repo.LogInUsuarioAsync(model.Email, model.Pass);
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                SigningCredentials credentials = new SigningCredentials(helperService.GetKeyToken(), SecurityAlgorithms.HmacSha256);

                UsuariosCubo userModel = new UsuariosCubo
                {
                    IdUsuario = user.IdUsuario,
                    Nombre = user.Nombre,
                    Email = user.Email,
                    Pass = user.Pass,
                    Imagen = user.Imagen
                };

                string jsonEmp = JsonConvert.SerializeObject(userModel);
                jsonEmp = helperCrypt.Encrypt(jsonEmp, vault.CryptExamen);
                Claim[] inf = new[]
                {
                    new Claim("UserData", jsonEmp)
                };

                JwtSecurityToken token = new JwtSecurityToken
                    (
                        claims: inf,
                        issuer: helperService.Issuer,
                        audience: helperService.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(20),
                        notBefore: DateTime.UtcNow
                    );

                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }
    }
}
