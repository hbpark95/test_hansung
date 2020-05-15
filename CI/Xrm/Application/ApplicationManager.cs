using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CI.Xrm
{
	public class ApplicationManager
	{
		private const string ORG_PROXY = "OrganizationServiceProxy";
		private string _conStr = string.Empty;

		public ApplicationManager(string conStr)
		{
			_conStr = conStr;
		}

		public OrganizationServiceProxy GetOrganizationServiceProxy()
		{
			try
			{
				OrganizationServiceProxy orgProxy = null;

				if (HttpContext.Current.Application[ORG_PROXY] == null)
					SetConnection(_conStr);
			
				Dictionary<string, object> obj = (Dictionary<string, object>)HttpContext.Current.Application[ORG_PROXY];
				orgProxy = obj[ORG_PROXY] as OrganizationServiceProxy;

				return orgProxy;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void SetConnection(string conStr)
		{
			ConnectionManager connectionManager = new ConnectionManager(conStr);
			OrganizationServiceProxy orgProxy = connectionManager.GetOrganizationServiceProxy();
			Dictionary<string, object> connectionDic = new Dictionary<string, object>();
			connectionDic.Add(ORG_PROXY,orgProxy);
			SetApplication(ORG_PROXY, connectionDic);
		}

		private void SetApplication(string key, object data)
		{
			if (HttpContext.Current.Application[key] == null)
			{
				HttpContext.Current.Application.Lock();
				HttpContext.Current.Application[key] = data;
				HttpContext.Current.Application.UnLock();
			}
		}
	}
}
