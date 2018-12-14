using ApiSamples.Controllers.Standard.Base;
using ApiSamples.Database;
using Microsoft.AspNetCore.Mvc;

namespace ApiSamples.Controllers.Standard
{
    public class PositionController : BaseController
    {
        readonly DatabaseContext Db;

        public PositionController(
            DatabaseContext db
        )
        {
            Db = db;
        }
    }
}