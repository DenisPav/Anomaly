using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Tenancy.Db;
using Tenancy.Services.Tenants;

namespace Tenancy.Services.Middlewares
{
    public class TenantResolveMiddleware
    {
        public RequestDelegate Next { get; set; }

        public TenantResolveMiddleware(RequestDelegate next)
        {
            this.Next = next;
        }

        public async Task Invoke(HttpContext ctx, ITenantResolver tenantResolver, DatabaseContext db)
        {
            var host = ctx.Request.GetTypedHeaders().Host.Value;
            var tenant = await tenantResolver.Resolve(host);

            if (tenant == null)
            {
                throw new Exception("Tenant not found");
            }
            else
            {
                db.SetActiveTenant(tenant);
                await Next(ctx);
            }
        }
    }
}