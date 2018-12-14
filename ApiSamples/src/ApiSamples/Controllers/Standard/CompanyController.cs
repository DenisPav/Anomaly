using ApiSamples.Controllers.Standard.Base;
using ApiSamples.Database;
using Microsoft.AspNetCore.Mvc;

namespace ApiSamples.Controllers.Standard
{
    public class CompanyController : BaseController
    {
        readonly DatabaseContext Db;

        public CompanyController(
            DatabaseContext db
        )
        {
            Db = db;
        }
    }
}