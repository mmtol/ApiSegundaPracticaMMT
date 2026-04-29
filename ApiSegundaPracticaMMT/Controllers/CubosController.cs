using ApiSegundaPracticaMMT.Helpers;
using ApiSegundaPracticaMMT.Models;
using ApiSegundaPracticaMMT.Repositories;
using ApiSegundaPracticaMMT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiSegundaPracticaMMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private RepositoryCubos repo;
        private HelperUsuarioToken helper;
        private ServiceStorageBlobs service;

        public CubosController(RepositoryCubos repo, HelperUsuarioToken helper, ServiceStorageBlobs service)
        {
            this.repo = repo;
            this.helper = helper;
            this.service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cubo>>> GetCubos()
        {
            return await repo.GetCubosAsync();
        }

        [HttpGet]
        [Route("{marca}")]
        public async Task<ActionResult<List<Cubo>>> GetCubosMarca(string marca)
        {
            return await repo.GetCubosMarcaAsync(marca);
        }

        [HttpPost("registro")]
        public async Task<ActionResult> Registro(UsuariosCubo user)
        {
            await repo.PostUsuarioAsync(user.Nombre, user.Email, user.Pass, user.Imagen);
            return Ok();
        }

        [HttpPost("registroblobstorage")]
        public async Task<ActionResult> RegistroBlobStorage(string nombre, string email, string pass, IFormFile fichero)
        {
            await PostBlobAsync("segundapracticausuarios", fichero);
            ModelStorage blob = await service.FindBlobAsync("segundapracticausuarios", fichero.FileName);
            await repo.PostUsuarioAsync(nombre, email, pass, blob.Uri);
            return Ok();
        }

        private async Task PostBlobAsync(string container, IFormFile fichero)
        {
            string blob = fichero.FileName;
            using (Stream stream = fichero.OpenReadStream())
            {
                await service.PostBlobAsync(container, blob, stream);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<UsuariosCubo>> FindUsuario()
        {
            UsuariosCubo usuario = helper.GetUsuario();
            return usuario;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<CompraCubos>>> GetPedidosUsuario()
        {
            UsuariosCubo usuario = helper.GetUsuario();
            return await repo.GetComprasCubosUsuarioAsync(usuario.IdUsuario);
        }

        [Authorize]
        [HttpPost("pedido")]
        public async Task<ActionResult> PostPedido(int idCubo)
        {
            UsuariosCubo usuarios = helper.GetUsuario();
            await repo.PostPedidoAsync(idCubo, usuarios.IdUsuario, DateTime.Now);
            return Ok();
        }
    }
}
