using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HANSUNG.Core.Models
{
    public class ViewModelContactList : DataResultModel
    {
        public ViewModelContactList()
        {
            this.data = new DataContactListModel();
        }

        public bool result { get; set; }
        public DataContactListModel data { get; set; }

    }

    public class DataContactListModel
    {
        public DataContactListModel()
        {
            contents = new ToastGridSubContact();
            pagination = new Pagination();
        }

        public ToastGridSubContact contents { get; set; }
        public Pagination pagination { get; set; }
    }

    public class ToastGridSubContact
    {
        public ToastGridSubContact()
        {
            contactList = new List<SubContact>();
        }
        public List<SubContact> contactList { get; set; }
    }
}
