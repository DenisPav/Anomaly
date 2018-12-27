using System.Threading.Tasks;
using Tenancy.Models;

namespace Tenancy.Services.Tenants
{
    public interface ITenantResolver
    {
        Task<Tenant> Resolve(string host);
    }
}