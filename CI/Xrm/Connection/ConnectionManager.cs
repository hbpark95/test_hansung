using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace CI.Xrm
{
	public class ConnectionManager
	{
		private string _conStr = string.Empty;

		public ConnectionManager(string conStr)
		{
			_conStr = conStr;
		}

		public OrganizationServiceProxy GetOrganizationServiceProxy(bool IsEarlyBound=true)
		{
            CrmServiceClient client = new CrmServiceClient(_conStr);

            //CrmServiceClient client = new CrmServiceClient("wooseok@citeam.onmicrosoft.com", ConvertToSecureString("adminci5454!@#"), "NorthAmerica",
            //    "orge09f7247",
            //    isOffice365: true);

            //CrmServiceClient(string crmUserId, SecureString crmPassword, string crmRegion, string orgName, bool useUniqueInstance = false, 
            //bool useSsl = false, OrganizationDetail orgDetail = null, bool isOffice365 = false);

            OrganizationServiceProxy orgProxy = client.OrganizationServiceProxy;

			/*
			 * Dynamics CRM 내부에 ISV 페이지를 접근할 때 가끔 발생하는 인증 오류
			 * 오류 메시지 : At least one security token in the message could not be validated.
			 * 해결 방안 : 우선 아래의 코드 한줄를 추가함으로써 에러가 재발하는지 모니터링해 보도록 한다.
			 * 모니터링 결과 : 코드 추가한 후에도 아주 가끔 오류가 재발하고 있음.
			 * */
			orgProxy.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.PeerTrust;

			if (IsEarlyBound == true)
				orgProxy.EnableProxyTypes();

			return orgProxy;
		}

        private System.Security.SecureString ConvertToSecureString(string password)
        {
            if (password == null)
                throw new ArgumentNullException("missing pwd");

            var securePassword = new System.Security.SecureString();
            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }

    }
}
