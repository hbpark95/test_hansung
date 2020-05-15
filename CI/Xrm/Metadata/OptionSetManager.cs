using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace CI.Xrm
{
	public class OptionSetManager
	{
		private const int _languageCode = 1042;
		private string _solutionName = string.Empty;
		private IOrganizationService _orgService = null;

		public OptionSetManager(OrganizationServiceProxy orgProxy, string solutionName = null)
		{
			IOrganizationService orgService = (IOrganizationService)orgProxy;
			_orgService = (IOrganizationService)orgService;
			_solutionName = solutionName;   // 솔루션 이름
		}

		/// <summary>
		/// Global OptionSet이 존재하는지 여부 체크 (존재하면 true 리턴)
		/// </summary>
		/// <param name="optionSetName"></param>
		/// <returns></returns>
		public bool IsExistGlobalOptionSet(string optionSetName)
		{
			bool retVal = true;

			RetrieveOptionSetRequest retrieveOptionSetRequest = new RetrieveOptionSetRequest
			{
				Name = optionSetName
			};

			string retText = string.Empty;
			try
			{
				// Execute the request.
				RetrieveOptionSetResponse retrieveOptionSetResponse = (RetrieveOptionSetResponse)_orgService.Execute(retrieveOptionSetRequest);
			}
			catch (Exception ex)
			{
				if (ex.Message.Equals("Could not find optionset"))
					retVal = false;
				else
					throw;
			}

			return retVal;
		}

		/// <summary>
		/// 옵션집합 가져오기
		/// </summary>
		/// <param name="optionSetName"></param>
		/// <returns></returns>
		public DtoOptionSet RetrieveGlobalOptionSet(string optionSetName)
		{
			try
			{
				RetrieveOptionSetRequest retrieveOptionSetRequest = new RetrieveOptionSetRequest
				{
					Name = optionSetName
				};

				// Execute the request.
				RetrieveOptionSetResponse retrieveOptionSetResponse = (RetrieveOptionSetResponse)_orgService.Execute(retrieveOptionSetRequest);

				//Console.WriteLine("Retrieved {0}.", retrieveOptionSetRequest.Name);

				// Access the retrieved OptionSetMetadata.
				OptionSetMetadata retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

				DtoOptionSet retVal = new DtoOptionSet
				{
					SchemaName = optionSetName,
					DisplayName = retrievedOptionSetMetadata.DisplayName.UserLocalizedLabel.Label,
					Description = retrievedOptionSetMetadata.Description.UserLocalizedLabel.Label
				};

				return retVal;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// 옵션집합의 Options 가져오기
		/// </summary>
		/// <param name="optionSetName"></param>
		/// <returns></returns>
		private OptionMetadata[] RetrieveGlobalOptionSetOptions(string optionSetName)
		{
			try
			{
				// Use the RetrieveOptionSetRequest message to retrieve  
				// a global option set by it's name.
				RetrieveOptionSetRequest retrieveOptionSetRequest = new RetrieveOptionSetRequest
				{
					Name = optionSetName
				};

				// Execute the request.
				RetrieveOptionSetResponse retrieveOptionSetResponse = (RetrieveOptionSetResponse)_orgService.Execute(retrieveOptionSetRequest);

				//Console.WriteLine("Retrieved {0}.", retrieveOptionSetRequest.Name);

				// Access the retrieved OptionSetMetadata.
				OptionSetMetadata retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

				// Get the current options list for the retrieved attribute.
				OptionMetadata[] options = retrievedOptionSetMetadata.Options.ToArray();

				return options;
			}
			catch(Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Global OptionSet 생성
		/// </summary>
		/// <param name="dto"></param>
		public void CreateGlobalOptionSet(DtoOptionSet dto)
		{
			try
			{
				OptionSetMetadata setupOptionSetMetadata = GetSetupOptionSetMetadata(dto);

				// Wrap the OptionSetMetadata in the appropriate request.
				CreateOptionSetRequest createOptionSetRequest = new CreateOptionSetRequest
				{
					SolutionUniqueName = !string.IsNullOrEmpty(_solutionName) ? _solutionName : null,
					OptionSet = setupOptionSetMetadata
				};

				// Pass the execute statement to the CRM service.
				OrganizationResponse responseFromUpdateOptionSet = _orgService.Execute(createOptionSetRequest);
	
				//foreach (var r in response.Results)
				//{
				//	Console.WriteLine(r.Value.ToString());
				//}

				OrganizationResponse responseFromOptions = InsertOrUpdateForOptionSetOptions(dto);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Global OptionSet 업데이트
		/// </summary>
		/// <param name="dto"></param>
		public void UpdateGlobalOptionSet(DtoOptionSet dto)
		{
			try
			{
				OptionSetMetadata setupOptionSetMetadata = GetSetupOptionSetMetadata(dto);

				// Wrap the OptionSetMetadata in the appropriate request.
				UpdateOptionSetRequest updateOptionSetRequest = new UpdateOptionSetRequest
				{
					OptionSet = setupOptionSetMetadata
				};

				// Pass the execute statement to the CRM service.
				OrganizationResponse responseFromUpdateOptionSet = _orgService.Execute(updateOptionSetRequest);

				OrganizationResponse responseFromOptions = InsertOrUpdateForOptionSetOptions(dto);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Global OptionSet 삭제
		/// </summary>
		/// <param name="schemaName"></param>
		public void DeleteGlobalOptionSet(string schemaName)
		{
			try
			{
				DeleteOptionSetRequest deleteOptionSetRequest = new DeleteOptionSetRequest
				{
					Name = schemaName
				};

				_orgService.Execute(deleteOptionSetRequest);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Global OptionSet의 Options 삭제
		/// </summary>
		/// <param name="optionSetName"></param>
		/// <param name="optionValue"></param>
		public void DeleteGlobalOptions(string optionSetName, int optionValue)
		{
			try
			{
				DeleteOptionValueRequest deleteOptionValueRequest =
					new DeleteOptionValueRequest
					{
						OptionSetName = optionSetName,
						Value = optionValue
					};

				_orgService.Execute(deleteOptionValueRequest);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Global OptionSet 세팅 값 가져오기
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		private OptionSetMetadata GetSetupOptionSetMetadata(DtoOptionSet dto)
		{
			OptionSetMetadata setupOptionSetMetadata = new OptionSetMetadata()
			{
				Name = dto.SchemaName,
				DisplayName = new Label(dto.DisplayName, _languageCode),
				Description = new Label(dto.Description ?? string.Empty, _languageCode),
				IsGlobal = true,
				OptionSetType = OptionSetType.Picklist,
			};

			return setupOptionSetMetadata;
		}

		/// <summary>
		/// Global OptionSet의 Options 값을 Insert or Update
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		private OrganizationResponse InsertOrUpdateForOptionSetOptions(DtoOptionSet dto)
		{
			try
			{
				OrganizationResponse response = null;
				OptionMetadata[] options = RetrieveGlobalOptionSetOptions(dto.SchemaName);

				// Add a label and value to Global OptionSet
				foreach (var o in dto.Options)
				{
					var optionCnt = options.Where(a => a.Value == o.Value).Count();

					if (optionCnt == 0)
					{
						InsertOptionValueRequest insertOptionValueRequest = new InsertOptionValueRequest
						{
							OptionSetName = dto.SchemaName,
							Value = o.Value, // optional value - if ommited one will get assigned automatically
							Label = new Label(o.Label, _languageCode),
							Description = new Label(o.Description ?? string.Empty, _languageCode)
						};

						response = _orgService.Execute(insertOptionValueRequest);
						//	int retVal = ((InsertOptionValueResponse)_orgService.Execute(insertOptionValueRequest)).NewOptionValue;
					}
					else // (optionCnt > 0)
					{
						UpdateOptionValueRequest updateOptionValueRequest = new UpdateOptionValueRequest
						{
							OptionSetName = dto.SchemaName,
							Value = o.Value, // optional value - if ommited one will get assigned automatically
							Label = new Label(o.Label, _languageCode),
							Description = new Label(o.Description ?? string.Empty, _languageCode)
						};

						response = _orgService.Execute(updateOptionValueRequest);
					}
				}

				return response;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// OptionSet의 <Label,Key> 가져오기
		/// </summary>
		/// <param name="logicalName"></param>
		/// <param name="optionSetName"></param>
		/// <returns></returns>
		public Dictionary<string, int> GetOptionSetValue(string logicalName, string optionSetName)
		{
			Dictionary<string, int> optionSet = new Dictionary<string, int>();

			var attReq = new RetrieveAttributeRequest();
			attReq.EntityLogicalName = logicalName;
			attReq.LogicalName = optionSetName;
			attReq.RetrieveAsIfPublished = true;
			
			var attResponse = (RetrieveAttributeResponse)_orgService.Execute(attReq);
			var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;

			var options = attMetadata.OptionSet.Options;
			foreach (var data in options)
			{
				optionSet.Add(data.Label.UserLocalizedLabel.Label, (int)data.Value);
			}

			return optionSet;
		}

		/// <summary>
		/// OptionSet의 <Key,Label> 가져오기
		/// </summary>
		/// <param name="logicalName"></param>
		/// <param name="optionSetName"></param>
		/// <returns></returns>
		public Dictionary<int, string> GetOptionSetText(string logicalName, string optionSetName)
		{
			Dictionary<int, string> optionSet = new Dictionary<int, string>();

			var attReq = new RetrieveAttributeRequest();
			attReq.EntityLogicalName = logicalName;
			attReq.LogicalName = optionSetName;
			attReq.RetrieveAsIfPublished = true;

			var attResponse = (RetrieveAttributeResponse)_orgService.Execute(attReq);
			var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;

			var options = attMetadata.OptionSet.Options;
			foreach (var data in options)
			{
				optionSet.Add((int)data.Value, data.Label.UserLocalizedLabel.Label);
			}

			return optionSet;
		}

		/// <summary>
		/// Global OptionSet의 <Label,Key> 가져오기
		/// </summary>
		/// <param name="optionSetName"></param>
		/// <returns></returns>
		public Dictionary<string, int> GetGlobalOptionSetValue(string optionSetName)
		{
			Dictionary<string, int> optionSet = new Dictionary<string, int>();

			var attReq = new RetrieveOptionSetRequest
			{
				Name = optionSetName
			};

			var optResponse = (RetrieveOptionSetResponse)_orgService.Execute(attReq);
			var optMetadata = (OptionSetMetadata)optResponse.OptionSetMetadata;

			var options = optMetadata.Options;
			foreach (var data in options)
			{
				optionSet.Add(data.Label.UserLocalizedLabel.Label, (int)data.Value);
			}

			return optionSet;
		}

		/// <summary>
		/// Global OptionSet의 <Key,Label> 가져오기
		/// </summary>
		/// <param name="xrm"></param>
		/// <param name="optionSetName"></param>
		/// <returns></returns>
		public Dictionary<int, string> GetGlobalOptionSetText(string optionSetName)
		{
			Dictionary<int, string> optionSet = new Dictionary<int, string>();

			var attReq = new RetrieveOptionSetRequest
			{
				Name = optionSetName
			};

			var optResponse = (RetrieveOptionSetResponse)_orgService.Execute(attReq);
			var optMetadata = (OptionSetMetadata)optResponse.OptionSetMetadata;

			var options = optMetadata.Options;
			foreach (var data in options)
			{
				optionSet.Add((int)data.Value, data.Label.UserLocalizedLabel.Label);
			}

			return optionSet;
		}
	}
}
