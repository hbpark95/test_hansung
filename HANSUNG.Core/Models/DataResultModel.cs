using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HANSUNG.Core.Models
{
    public class DataResultModel
    {
        /// <summary>
        /// S : 성공
        /// F : 실패
        /// </summary>
        public string ResultKey { get; set; }
        public string ResultMessage { get; set; }
    }
}
