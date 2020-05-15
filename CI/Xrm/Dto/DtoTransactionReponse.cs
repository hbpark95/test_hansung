using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Xrm
{
	public class DtoTransactionResponse
	{
		public int ErrorCode { get; set; }
		public ErrorDetailCollection ErrorDetails { get; set; }
		public string Message { get; set; }
	}

	public class DtoTransactionResponseGuid
	{
		public string Id { get; set; }
	}
}
