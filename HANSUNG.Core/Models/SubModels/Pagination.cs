using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HANSUNG.Core.Models
{
    public class Pagination
    {
        /// <summary>
        /// 현재 페이지
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 모든 로우들의 개수
        /// </summary>
        public int totalCount { get; set; }
    }
}
