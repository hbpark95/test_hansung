using HANSUNG.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HANSUNG.Common
{
    public class BaseController : Controller
    {
        public ContactService _contactService = new ContactService();
    }
}