using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Xrm
{
	public class MultipleRequestManager
	{
		private const int MAX_CNT = 1000;
		private int _listCount = 0;
		private int _currentCount = 0;
		private bool _continueOnError;
		private bool _returnResponses;
		private OrganizationServiceProxy _orgProxy = null;
		private List<ExecuteMultipleRequest> _multipleRequestList = new List<ExecuteMultipleRequest>();
		private ExecuteMultipleRequest _multipleRequest;

		public MultipleRequestManager(OrganizationServiceProxy orgProxy,bool continueOnError = false, bool returnResponses = true)
		{
			_continueOnError = continueOnError;
			_returnResponses = returnResponses;

			_orgProxy = orgProxy;
			InitMultipleRequest();
		}

		public List<ExecuteMultipleRequest> GetMultipleRequestList()
		{
			return _multipleRequestList;
		}

		private void InitMultipleRequest()
		{
			_multipleRequest = new ExecuteMultipleRequest()
			{
				Settings = new ExecuteMultipleSettings()
				{
					ContinueOnError = _continueOnError,
					ReturnResponses = _returnResponses
				},
				Requests = new OrganizationRequestCollection()
			};
		}

		public void SetMultipleRequest(OrganizationRequest item)
		{
			int internalCount = _multipleRequest.Requests.Count;
			_currentCount = _currentCount + 1;
			_listCount = _currentCount / MAX_CNT;		// List Count -> List에 1000개가 등록되었을 경우 count 1로 된다. 1000개가 넘어가면 count 2 처리됨.

			if (internalCount == MAX_CNT)
			{
				_multipleRequestList.Add(_multipleRequest);
				InitMultipleRequest();
			}
			
			_multipleRequest.Requests.Add(item);
		}

		public List<ExecuteMultipleRequest> SetLastMultipleRequest()
		{
			if (_multipleRequest.Requests.Count > 0)
			{
				_multipleRequestList.Add(_multipleRequest);
			}

			return _multipleRequestList;
		}

		public List<DtoMultipleReponse> Execute(List<ExecuteMultipleRequest> resultList)
		{
			try
			{
				List<DtoMultipleReponse> errorResponseList = new List<DtoMultipleReponse>();

				for (int i = 0; i < resultList.Count; i++)
				{
					ExecuteMultipleRequest request = resultList[i] as ExecuteMultipleRequest;
					ExecuteMultipleResponse responseResult = (ExecuteMultipleResponse)_orgProxy.Execute(request);
					Console.WriteLine(string.Format("ExecuteMultipleRequest Execute - Trying - {0}",i+1));

					foreach (var responseItem in responseResult.Responses)
					{
						if (responseItem.Response != null)
						{
						}
						else if (responseItem.Fault != null)
						{
                            DtoMultipleReponse dto = new DtoMultipleReponse() {
                                IsSuccess = false,
                                RequestId = ((Entity)request.Requests[responseItem.RequestIndex].Parameters["Target"]).Id.ToString(),
                                RequestIndex = responseItem.RequestIndex,
                                RequestName = request.Requests[responseItem.RequestIndex].RequestName,
                                RequestFaultMessage = responseItem.Fault.Message };
							errorResponseList.Add(dto);
						}
					}
				}

				foreach (var e in errorResponseList)
				{
					if (e.IsSuccess == false)
					{
						Console.WriteLine(string.Format("[ERROR] RequestFaultMessage : {0}", e.RequestFaultMessage));
						Console.WriteLine(string.Format("[ERROR] RequestId : {0}", e.RequestId));
						Console.WriteLine(string.Format("[ERROR] RequestIndex : {0}", e.RequestIndex));
						Console.WriteLine(string.Format("[ERROR] RequestName : {0}", e.RequestName));
					}
				}

				return errorResponseList;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
