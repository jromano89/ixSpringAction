using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Intelledox.Extension;
using Intelledox.Extension.Action;
using Intelledox.Model;
using Intelledox.QAWizard;
using Newtonsoft.Json.Linq;
using NLog;

namespace SpringCMWorkflow
{
    public class InitiateWorkflow : ActionConnector
    {
        public override ExtensionIdentity ExtensionIdentity { get; protected set; } = new ExtensionIdentity
        {
            Id = new Guid("78AF4DDF-0780-489F-A02D-8A4D94B373C2"),
            Name = "Spring CM Workflow"
        };

        public override List<AvailableInput> GetAvailableInputs()
        {
            return new List<AvailableInput>
            {
                new AvailableInput
                {
                    Id = this._xmlDocumentName,
                    Name = "XML Document Name",
                    InstanceLimit = 1,
                    Required = true
                },
                new AvailableInput
                {
                    Id = this._workflowName,
                    Name = "Workflow Name",
                    InstanceLimit = 1,
                    Required = true
                },
                new AvailableInput
                {
                    Id = this._environment,
                    Name = "Environment",
                    ValueType = Intelledox.Extension.Action.ValueTypes.Enum,
                    EnumValues = new string[]
                    {
                        "prod",
                        "uat"
                    }
                }
            };
        }

        public override List<AvailableOutput> GetAvailableOutputs()
        {
            return new List<AvailableOutput>()
            {
                new AvailableOutput()
                {
                    Id = _workflowID,
                    Name = "Workflow ID"
                }
            };
        }

        public override GlobalSettingIdentity GlobalSettingIdentity { get; protected set; } = SpringCMConnectorSettings.GetGlobalSettingIdentity();

        public override List<GlobalSetting> GetAvailableGlobalSettings()
        {
            return SpringCMConnectorSettings.GetAvailableGlobalSettings();
        }

        public override async Task<ActionResult> RunAsync(ActionProperties properties)
        {
            ActionResult actionResult = new ActionResult
            {
                Result = 0
            };
            try
            {
                SpringCMConnectorSettings settings = new SpringCMConnectorSettings(properties.Context.BusinessUnitGuid);
                bool debug = settings.DebugMode;
                string clientId = string.Empty;
                string clientSecret = string.Empty;
                string authURL = string.Empty;
                string requestURL = string.Empty;
                string xmlDocumentName = string.Empty;
                string workflowName = string.Empty;
                string environment = string.Empty;
                string xmlPayload = string.Empty;

                foreach (RoutingOption routingOption in properties.ActionInputs)
                {
                    ActionInput input = (ActionInput)routingOption;
                    bool flag5 = input.ElementTypeId == this._xmlDocumentName;
                    if (flag5)
                    {
                        xmlDocumentName = input.OutputValue;
                        bool flag6 = string.IsNullOrEmpty(xmlDocumentName);
                        if (flag6)
                        {
                            throw new ApplicationException("An XML document name must be specified.  Please check your action attributes in Desiger.");
                        }
                        bool xmlFound = false;
                        for (int i = 0; i < properties.Documents.Count; i++)
                        {
                            string extension = properties.Documents[i].Extension;
                            if (!(extension == ".xml"))
                            {
                                throw new Exception("XML document not present.  Check Design to ensure an XML document is created.");
                            }
                            bool flag7 = properties.Documents[i].DisplayName.Equals(xmlDocumentName);
                            if (flag7)
                            {
                                string text = await this.GetDocumentTextAsync(properties, properties.Documents[i]);
                                xmlPayload = text;
                                text = null;
                                xmlFound = true;
                            }
                            if (xmlFound)
                            {
                                break;
                            }
                        }
                    }
                    else if (input.ElementTypeId == this._workflowName)
                    {
                        workflowName = input.OutputValue;
                        if (string.IsNullOrEmpty(workflowName))
                        {
                            throw new ApplicationException("A SpringCM workflow name must be specified.  Please check your action attributes in Designer.");
                        }
                    }
                    else if (input.ElementTypeId == this._environment)
                    {
                        environment = input.OutputValue;
                        if (string.IsNullOrEmpty(environment))
                        {
                            throw new ApplicationException("A SpringCM environment must be specified.  Please check your action attributes in Designer.");
                        }
                    }
                    input = null;
                }

                if (environment == "prod")
                {
                    clientId = settings.ClientId;
                    clientSecret = settings.ClientSecret;
                    authURL = settings.AuthURL;
                    requestURL = settings.RequestURL;
                }
                else
                {
                    clientId = settings.UatClientId;
                    clientSecret = settings.UatClientSecret;
                    authURL = settings.UatAuthURL;
                    requestURL = settings.UatRequestURL;
                }

                bool flag = string.IsNullOrEmpty(clientId);
                if (flag)
                {
                    throw new Exception("Infiniti user name must be provided.  Please fix the connector settings in Manage.  Manage -> Settings -> Connector Settings -> SpringCM Connector");
                }
                bool flag2 = string.IsNullOrEmpty(clientSecret);
                if (flag2)
                {
                    throw new Exception("Infiniti password name must be provided.  Please fix the connector settings in Manage.  Manage -> Settings -> Connector Settings -> SpringCM Connector");
                }
                bool flag3 = string.IsNullOrEmpty(authURL);
                if (flag3)
                {
                    throw new Exception("An endpoint must be provided.  Please fix the connector settings in Manage.  Manage -> Settings -> Connector Settings -> SpringCM Connector");
                }
                bool flag4 = string.IsNullOrEmpty(requestURL);
                if (flag4)
                {
                    throw new Exception("An endpoint must be provided.  Please fix the connector settings in Manage.  Manage -> Settings -> Connector Settings -> SpringCM Connector");
                }

                HttpClientHandler handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);
                client.DefaultRequestHeaders.ExpectContinue = new bool?(false);
                JObject jobject = new JObject();
                jobject.Add("client_id", clientId);
                jobject.Add("client_secret", clientSecret);
                JObject login = jobject;
                if (debug)
                {
                    this.Log("JSON payload: " + login.ToString(), properties.Context.UserGuid, LogLevel.Info);
                    properties.AddMessage("JSON payload: " + login.ToString(), "SpringCM Initiate Workflow");
                }
                HttpResponseMessage httpResponseMessage = await client.PostAsync(authURL, new StringContent(login.ToString(), Encoding.UTF8, "application/json"));
                HttpResponseMessage authResponse = httpResponseMessage;
                httpResponseMessage = null;
                if (!authResponse.IsSuccessStatusCode)
                {
                    string text2 = await authResponse.Content.ReadAsStringAsync();
                    string authResponseContent = text2;
                    text2 = null;
                    this.Log(string.Concat(new object[]
                    {
                        "Could not make login web service call.  Response status code: ",
                        authResponse.StatusCode,
                        " response content",
                        authResponseContent,
                        " response reason phrase",
                        authResponse.ReasonPhrase
                    }), properties.Context.UserGuid, LogLevel.Info);
                    authResponseContent = null;
                }
                string bearerToken = string.Empty;
                string text3 = await authResponse.Content.ReadAsStringAsync();
                JObject token = JObject.Parse(text3);
                text3 = null;
                bearerToken = token["access_token"].ToString();
                JObject jobject2 = new JObject();
                jobject2.Add("Name", workflowName);
                jobject2.Add("Params", xmlPayload);
                JObject workflow_config = jobject2;
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);
                HttpResponseMessage httpResponseMessage2 = await client.PostAsync(requestURL, new StringContent(workflow_config.ToString(), Encoding.UTF8, "application/json"));
                HttpResponseMessage generateResponse = httpResponseMessage2;
                httpResponseMessage2 = null;
                string text4 = await generateResponse.Content.ReadAsStringAsync();
                string generateResponseContent = text4;
                text4 = null;
                if (!generateResponse.IsSuccessStatusCode)
                {
                    this.Log(string.Concat(new object[]
                    {
                        "Could not make request web service call.  Response status code: ",
                        generateResponse.StatusCode,
                        " response content",
                        generateResponseContent,
                        " response reason phrase",
                        generateResponse.ReasonPhrase
                    }), properties.Context.UserGuid, LogLevel.Info);
                    properties.AddMessage(string.Concat(new object[]
                    {
                        "Could not make request web service call.  Response status code: ",
                        generateResponse.StatusCode,
                        " response content",
                        generateResponseContent,
                        " response reason phrase",
                        generateResponse.ReasonPhrase
                    }), "Infiniti REST Action");
                    throw new Exception("Error calling SpringCM");
                }

                actionResult.Result = Intelledox.QAWizard.Design.ActionResultType.Success;
                JObject jsonResponse = JObject.Parse(generateResponseContent);
                string href = jsonResponse["Href"].ToString();
                string workflowID = href.Substring(href.Length - 36);
                actionResult.Outputs = new List<Intelledox.Model.ActionOutput> {
                    new Intelledox.Model.ActionOutput()
                    {
                      ID = _workflowID,
                      Name = "Workflow ID",
                      Value = workflowID
                    }
                };
                settings = null;
                clientId = null;
                clientSecret = null;
                authURL = null;
                requestURL = null;
                xmlDocumentName = null;
                workflowName = null;
                xmlPayload = null;
                handler = null;
                client = null;
                login = null;
                authResponse = null;
                bearerToken = null;
                token = null;
                workflow_config = null;
                generateResponse = null;
            }
            catch (ArgumentNullException ane)
            {
                properties.AddMessage("ArgumentNullException " + ane.Message, "SpringCM Initiate Workflow");
                properties.AddMessage("ArgumentNullException " + ane.InnerException.ToString(), "SpringCM Initiate Workflow");
                actionResult.Result = Intelledox.QAWizard.Design.ActionResultType.Fail;
            }
            catch (HttpRequestException hre)
            {
                properties.AddMessage("HttpRequestException " + hre.Message, "SpringCM Initiate Workflow");
                properties.AddMessage("HttpRequestException " + hre.InnerException.ToString(), "SpringCM Initiate Workflow");
                actionResult.Result = Intelledox.QAWizard.Design.ActionResultType.Fail;
            }
            catch (Exception ex)
            {
                properties.AddMessage("Exception " + ex.Message, "SpringCM Initiate Workflow");
                properties.AddMessage("Exception " + ex.Message.ToString(), "SpringCM Initiate Workflow");
                actionResult.Result = Intelledox.QAWizard.Design.ActionResultType.Fail;
            }
            return actionResult;
        }

        public override bool SupportsRun()
        {
            return true;
        }

        public override bool SupportsUI()
        {
            return false;
        }

        private void Log(string message, Guid UserGuid, LogLevel level)
        {
            LogEventInfo logEventInfo = new LogEventInfo();
            logEventInfo.Level = level;
            logEventInfo.Properties["dateCreatedUtc"] = DateTime.UtcNow;
            logEventInfo.Properties["userGuid"] = UserGuid;
            logEventInfo.Properties["extraDetails"] = message;
            logEventInfo.Message = "Send to Infiniti REST Service";
            this.Logger.Log(logEventInfo);
        }

        private async Task<string> GetDocumentTextAsync(ActionProperties properties, Document document)
        {
            Stream stream = await properties.GetDocumentStreamAsync(document);
            Stream documentStream = stream;
            stream = null;
            string result;
            try
            {
                using (StreamReader reader = new StreamReader(documentStream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                if (documentStream != null)
                {
                    ((IDisposable)documentStream).Dispose();
                }
            }
            return result;
        }

        private Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Guid _xmlDocumentName = new Guid("8A3081F4-62D6-4E5C-A9D9-2EB4792DA6BC");

        private readonly Guid _workflowName = new Guid("892EF5FA-3174-499E-ACA0-1CBA7B7F852F");

        private readonly Guid _environment = new Guid("956446C6-1F9E-4317-B936-3951838A348E");

        private readonly Guid _workflowID = new Guid("8FE0739D-ABB1-4600-B6C9-E149A05E418F");

    }
}
