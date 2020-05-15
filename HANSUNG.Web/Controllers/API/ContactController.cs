using HANSUNG.Common;
using HANSUNG.Core.Models;
using System.Web.Mvc;

namespace HANSUNG.Controllers.API
{
    public class ContactController : BaseController
    {
        public JsonResult GetContactList(RouteGetContactList route)
        {
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