using HANSUNG.Common;
using HANSUNG.Core.Models;
using NLog;
using System.Web.Mvc;

namespace HANSUNG.Controllers.API
{
    public class ContactController : BaseController
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public JsonResult GetContactList(RouteGetContactList route)
        {
            logger.Trace("GetContactList");

            ViewModelContactList vmodel = new ViewModelContactList();

            _contactService.GetContactList(ref vmodel, ref route);

            return Json(vmodel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContactList()
        {
            return View();
        }

        public ActionResult UpdateAllRecord()
        {

            return View();
        }
    }
}