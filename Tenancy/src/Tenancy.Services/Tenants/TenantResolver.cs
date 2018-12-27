using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenancy.Common.DI;
using Tenancy.Db;
using Tenancy.Models;

namespace Tenancy.Services.Tenants
{
    [TransientService]
    public class TenantResolver : ITenantResolver
    {
        readonly DatabaseContext Db;
        readonly ILogger<TenantResolver> Log;

        public TenantResolver(
            ILogger<TenantResolver> log,
            DatabaseContext db
        )
        {
            Log = log;
            Db = db;
        }

        public Task<Tenant> Resolve(string host)
        {
            Log.LogInformation("Trying to fetch tenant for host {host}", host);

            return Db.Tenants.FirstOrDefaultAsync(x => x.Host == host);
        }
    }
}