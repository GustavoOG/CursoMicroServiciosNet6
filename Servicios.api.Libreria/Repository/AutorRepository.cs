using MongoDB.Driver;
using Servicios.api.Libreria.Core.ContextMongoDB;
using Servicios.api.Libreria.Core.Entities;

namespace Servicios.api.Libreria.Repository
{
    public class AutorRepository : IAutorRepository
    {
        private readonly IAutorContext _autorContext;
        public AutorRepository(IAutorContext autorContext)
        {
            _autorContext = autorContext;
        }

        public async Task<IEnumerable<Autor>> GetAutores() => await _autorContext.Autores.FindSync(p => true).ToListAsync();


    }
}
