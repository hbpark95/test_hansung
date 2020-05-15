using CI.Xrm;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HANSUNG.Core.Common
{
    public class InitConfig

    {
        private Guid _callerId = Guid.Empty;
        private OrganizationServiceProxy _orgProxy;
        private XrmServiceContext _xrm;

        #region 속성 Get 함수
        public OrganizationServiceProxy OrgProxy
        {
            get { return _orgProxy; }
        }
        public XrmServiceContext Xrm
        {
            get { return _xrm; }
        }
        #endregion

        public InitConfig()
        {
            _orgProxy = CreateOrganizationServiceProxy();
            _xrm = new XrmServiceContext(_orgProxy);
        }

        public InitConfig(Guid callerId)
        {
            _callerId = callerId;
            _orgProxy = CreateOrganizationServiceProxy();
            _xrm = new XrmServiceContext(_orgProxy);
        }

        private OrganizationServiceProxy CreateOrganizationServiceProxy()
        {
            try
            {
                string conStr;
                conStr = ConfigurationManager.ConnectionStrings["CRMConnectionString"].ToString();
                OrganizationServiceProxy orgProxy = null;

                // 웹에서는 주석 풀기
                if (HttpContext.Current != null)
                {
                    ApplicationManager applicationManager = new ApplicationManager(conStr);
                    orgProxy = applicationManager.GetOrganizationServiceProxy();
                }
                else
                {
                    ConnectionManager connectionManager = new ConnectionManager(conStr);
                    orgProxy = connectionManager.GetOrganizationServiceProxy();
                }

                orgProxy.Timeout.Add(TimeSpan.FromSeconds(Double.Parse(ConfigurationManager.AppSettings["timeout"])));

                if (_callerId != Guid.Empty)
                    orgProxy.CallerId = _callerId;

                return orgProxy;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
