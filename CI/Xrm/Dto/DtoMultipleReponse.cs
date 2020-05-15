using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Xrm
{
	public class DtoMultipleReponse
	{
		public bool IsSuccess { get; set; }
		public string RequestId { get; set; }
		public int RequestIndex { get; set; }
		public int RequestListIndex { get; set; }
		public string RequestName { get; set; }
		public string RequestFaultMessage { get; set; }
	}
}
