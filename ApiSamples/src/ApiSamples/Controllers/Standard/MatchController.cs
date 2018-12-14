using ApiSamples.Controllers.Standard.Base;
using ApiSamples.Database;
using Microsoft.AspNetCore.Mvc;

namespace ApiSamples.Controllers.Standard
{
    public class MatchController : BaseController
    {
        readonly DatabaseContext Db;

        public MatchController(
            DatabaseContext db
        )
        {
            Db = db;
        }
    }
}