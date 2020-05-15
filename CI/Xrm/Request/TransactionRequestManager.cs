using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CI.Xrm
{
	public class TransactionRequestManager
	{
		private const int MAX_CNT = 1000;
		private int _listCount = 0;
		private int _currentCount = 0;
		private bool _continueOnError;
		private bool _returnResponses;
		private OrganizationServiceProxy _orgProxy = null;
		private List<ExecuteTransactionRequest> _transactionRequestList = new List<ExecuteTransactionRequest>();
		private ExecuteTransactionRequest _transactionRequest;

		public TransactionRequestManager(OrganizationServiceProxy orgProxy, bool continueOnError = false, bool returnResponses = true)
		{
			_continueOnError = continueOnError;
			_returnResponses = returnResponses;

			_orgProxy = orgProxy;
			InitTransactionRequest();
		}

		public List<ExecuteTransactionRequest> GetTransactionRequestList()
		{
			return _transactionRequestList;
		}

		private void InitTransactionRequest()
		{
			_orgProxy.EnableProxyTypes();

			_transactionRequest = new ExecuteTransactionRequest()
			{
				Requests = new OrganizationRequestCollection(),
				ReturnResponses = true
			};
		}

		public void SetTransactionRequest(OrganizationRequest item)
		{
			int internalCount = _transactionRequest.Requests.Count;
			_currentCount = _currentCount + 1;
			_listCount = _currentCount / MAX_CNT;       // List Count -> List에 1000개가 등록되었을 경우 count 1로 된다. 1000개가 넘어가면 count 2 처리됨.

			if (internalCount == MAX_CNT)
			{
				_transactionRequestList.Add(_transactionRequest);
				InitTransactionRequest();
			}

			_transactionRequest.Requests.Add(item);
		}

		public List<ExecuteTransactionRequest> SetLastTransactionRequest()
		{
			if (_transactionRequest.Requests.Count > 0)
			{
				_transactionRequestList.Add(_transactionRequest);
			}

			return _transactionRequestList;
		}

		public List<DtoTransactionResponseGuid> Execute(List<ExecuteTransactionRequest> resultList)
		{
			List<DtoTransactionResponse> errorResponseList = new List<DtoTransactionResponse>();
			List<DtoTransactionResponseGuid> responseResultList = new List<DtoTransactionResponseGuid>();

			try
			{
				for (int i = 0; i < resultList.Count; i++)
				{
					ExecuteTransactionRequest request = resultList[i] as ExecuteTransactionRequest;
					ExecuteTransactionResponse responseResult = (ExecuteTransactionResponse)_orgProxy.Execute(request);

					foreach (var resultResponse in responseResult.Responses)
					{
						if (resultResponse.ResponseName != "Delete")
						{
							responseResultList.Add(new DtoTransactionResponseGuid
							{
								Id = resultResponse.Results.Select(a => a.Value.ToString()).SingleOrDefault()
							});
						}
					}
				}

				return responseResultList;
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				DtoTransactionResponse dto = new DtoTransactionResponse()
				{
					ErrorDetails = ex.Detail.ErrorDetails,
					Message = ex.Detail.Message,
					ErrorCode = ex.Detail.ErrorCode
				};

				throw;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public List<DtoMultipleReponse> Execute(List<ExecuteTransactionRequest> resultList, bool isOnlyErrRes = true)
		{
			List<DtoMultipleReponse> errorResponseList = new List<DtoMultipleReponse>();
			try
			{
				for (int i = 0; i < resultList.Count; i++)
				{
					ExecuteTransactionRequest request = resultList[i] as ExecuteTransactionRequest;
					ExecuteTransactionResponse responseResult = (ExecuteTransactionResponse)_orgProxy.Execute(request);

					if (!isOnlyErrRes)
					{
						foreach (var responseItem in responseResult.Responses)
						{
							if (responseItem.Results.Count > 0)
							{
								DtoMultipleReponse dto = new DtoMultipleReponse()
								{
									IsSuccess = true,
									RequestName = responseItem.ResponseName
								};

								if ("Create".Equals(responseItem.ResponseName)) //Create만 Return Id가 존재하는 듯함
								{
									//dto.RequestId = responseItem.Results["id"].ToString();
								}
								errorResponseList.Add(dto);
							}
						}
					}
				}
				return errorResponseList;
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				DtoMultipleReponse dto = new DtoMultipleReponse()
				{
					IsSuccess = false,
					RequestFaultMessage = ex.Detail.Message
				};
				errorResponseList.Add(dto);

				return errorResponseList;
			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}