using Api.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz minimalista para Unit of Work.
    /// Solo garantiza SaveChangesAsync. Los repositorios se inyectan por separado en los servicios.
    /// </summary>
    public interface IUnitOfWork
    {
        // Guardar cambios - el único método de persistencia
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}