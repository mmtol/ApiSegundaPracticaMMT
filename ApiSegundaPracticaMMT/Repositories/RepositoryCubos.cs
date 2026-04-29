using ApiSegundaPracticaMMT.Data;
using ApiSegundaPracticaMMT.Models;
using ApiSegundaPracticaMMT.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiSegundaPracticaMMT.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext context;
        private ServiceStorageBlobs service;

        public RepositoryCubos(CubosContext context, ServiceStorageBlobs service)
        {
            this.context = context;
            this.service = service;
        }

        //getCubos
        //getCubosMarca
        //login
        //postUsuario
        //findUsuario
        //getPedidosUsuario
        //postPedido

        private async Task<int> GetMaxIdUsuariosAsync()
        {
            if (context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await context.Usuarios.MaxAsync(z => z.IdUsuario) + 1;
            }
        }

        private async Task<int> GetMaxIdPedidoAsync()
        {
            if (context.Compras.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await context.Compras.MaxAsync(z => z.IdPedido) + 1;
            }
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            List<Cubo> cubos = await context.Cubos.ToListAsync();
            foreach (Cubo cubo in cubos)
            {
                ModelStorage model = await service.FindBlobAsync("segundapracticacubos", cubo.Imagen);
                cubo.Imagen = model.Uri;
            }
            return cubos;
        }

        public async Task<List<Cubo>> GetCubosMarcaAsync(string marca)
        {
            List<Cubo> cubos = await context.Cubos.Where(x => x.Marca == marca).ToListAsync();
            foreach (Cubo cubo in cubos)
            {
                ModelStorage model = await service.FindBlobAsync("segundapracticacubos", cubo.Imagen);
                cubo.Imagen = model.Uri;
            }
            return cubos;
        }

        public async Task<UsuariosCubo> LogInUsuarioAsync(string email, string pass)
        {
            return await context.Usuarios.Where(x => x.Email == email && x.Pass == pass).FirstOrDefaultAsync();
        }

        public async Task PostUsuarioAsync(string nombre, string email, string pass, string imagen)
        {
            int id = await GetMaxIdUsuariosAsync();

            UsuariosCubo user = new UsuariosCubo
            {
                IdUsuario = id,
                Nombre = nombre,
                Email = email,
                Pass = pass,
                Imagen = imagen
            };

            await context.Usuarios.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<UsuariosCubo> FindUsuarioAsync(int id)
        {
            return await context.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == id);
        }

        public async Task<List<CompraCubos>> GetComprasCubosUsuarioAsync(int id)
        {
            return await context.Compras.Where(x => x.IdUsuario == id).ToListAsync();
        }

        public async Task PostPedidoAsync(int idCubo, int idUsuario, DateTime fechaPedido)
        {
            int id = await GetMaxIdPedidoAsync();

            CompraCubos compra = new CompraCubos
            {
                IdPedido = id,
                IdCubo = idCubo,
                IdUsuario = idUsuario,
                FechaPedido = fechaPedido
            };

            await context.Compras.AddAsync(compra);
            await context.SaveChangesAsync();
        }
    }
}
