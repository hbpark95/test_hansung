using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Xrm
{
	public class DtoOptionSet
	{
		public string DisplayName { get; set; }
		public string SchemaName { get; set; }
		public string Description { get; set; }
		public List<DtoOptions> Options { get; set; }
	}
}
