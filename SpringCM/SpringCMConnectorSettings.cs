using System;
using System.Collections.Generic;
using Intelledox.Controller;
using Intelledox.Extension;
using Intelledox.Model;

namespace SpringCMWorkflow
{
	internal class SpringCMConnectorSettings
	{
		public static GlobalSettingIdentity GetGlobalSettingIdentity()
		{
			return new GlobalSettingIdentity
			{
				Id = SpringCMConnectorSettings._SettingsType,
				Name = "SpringCM Connector"
			};
		}

		public static List<GlobalSetting> GetAvailableGlobalSettings()
		{
			return new List<GlobalSetting>
			{
				new GlobalSetting
				{
					Id = SpringCMConnectorSettings._Setting_ClientId,
					Name = "Client ID (Prod)",
					ObfuscateDisplay = false,
					SortOrder = 0
				},
				new GlobalSetting
				{
					Id = SpringCMConnectorSettings._Setting_ClientSecret,
					Name = "Client Secret (Prod)",
					ObfuscateDisplay = false,
					SortOrder = 1
				},
				new GlobalSetting
				{
					Id = SpringCMConnectorSettings._Setting_AuthUrl,
					Name = "Authorization URL (Prod)",
					ObfuscateDisplay = false,
					SortOrder = 2,
					DefaultValue = "https://auth.springcm.com/api/v201606/apiuser"
                },
				new GlobalSetting
				{
					Id = SpringCMConnectorSettings._Setting_RequestUrl,
					Name = "Request URL (Prod)",
					ObfuscateDisplay = false,
					SortOrder = 3,
                    DefaultValue = "https://apina11.springcm.com/v201411/workflows"
                },
                    new GlobalSetting
                {
                    Id = SpringCMConnectorSettings._Setting_UatClientId,
                    Name = "Client ID (UAT)",
                    ObfuscateDisplay = false,
                    SortOrder = 4
                },
                new GlobalSetting
                {
                    Id = SpringCMConnectorSettings._Setting_UatClientSecret,
                    Name = "Client Secret (UAT)",
                    ObfuscateDisplay = false,
                    SortOrder = 5
                },
                new GlobalSetting
                {
                    Id = SpringCMConnectorSettings._Setting_UatAuthUrl,
                    Name = "Authorization URL (UAT)",
                    ObfuscateDisplay = false,
                    SortOrder = 6,
                    DefaultValue = "https://authuat.springcm.com/api/v201606/apiuser"
                },
                new GlobalSetting
                {
                    Id = SpringCMConnectorSettings._Setting_UatRequestUrl,
                    Name = "Request URL (UAT)",
                    ObfuscateDisplay = false,
                    SortOrder = 7,
                    DefaultValue = "https://apiuatna11.springcm.com/v201411/workflows"
                },
                new GlobalSetting
				{
					Id = SpringCMConnectorSettings._Setting_DebugMode,
					Name = "Debug [true|false]",
					ObfuscateDisplay = false,
					SortOrder = 8,
					DefaultValue = "false"
				}
			};
		}

		public SpringCMConnectorSettings(Guid businessUnitGuid)
		{
			this.GetConnectorSettings(businessUnitGuid);
		}

        private void GetConnectorSettings(Guid businessUnitGuid)
        {
            ConnectorSettingsElementTypeCollection connectorSettingsElementTypes = ConnectorSettingsElementTypeController.GetConnectorSettingsElementTypes(businessUnitGuid, SpringCMConnectorSettings._SettingsType);
            this.ClientId = connectorSettingsElementTypes[SpringCMConnectorSettings._Setting_ClientId].ElementValue;
            this.ClientSecret = connectorSettingsElementTypes[SpringCMConnectorSettings._Setting_ClientSecret].ElementValue;
            this.AuthURL = connectorSettingsElementTypes[SpringCMConnectorSettings._Setting_AuthUrl].ElementValue;
            this.RequestURL = connectorSettingsElementTypes[SpringCMConnectorSettings._Setting_RequestUrl].ElementValue;
            this.UatClientId = connectorSettingsElementTypes[SpringCMConnectorSettings._Setting_UatClientId].ElementValue;
            this.UatClientSecret = connectorSettingsElementTypes[SpringCMConnectorSettings._Setting_UatClientSecret].ElementValue;
            this.UatAuthURL = connectorSettingsElementTypes[SpringCMConnectorSettings._Setting_UatAuthUrl].ElementValue;
            this.UatRequestURL = connectorSettingsElementTypes[SpringCMConnectorSettings._Setting_UatRequestUrl].ElementValue;
            this.DebugMode = bool.Parse(connectorSettingsElementTypes[SpringCMConnectorSettings._Setting_DebugMode].ElementValue);
        }

		private static Guid _SettingsType = new Guid("19757459-48A8-44A4-A8DD-1835B57B74F3");

		private static Guid _Setting_ClientId = new Guid("FE7A6A48-EBBA-5260-BA71-D4CEBE096373");

		private static Guid _Setting_ClientSecret = new Guid("5D29C300-39E8-4561-BABA-29207842176B");

		private static Guid _Setting_AuthUrl = new Guid("8E35946F-A6F6-561C-B4DF-CDCF5DD9BF0B");

		private static Guid _Setting_RequestUrl = new Guid("D13A6C0D-C087-527D-820F-4D3A797FC856");

        private static Guid _Setting_UatClientId = new Guid("FE7A6A48-EBBA-5260-BA71-D4CEBE096374");

        private static Guid _Setting_UatClientSecret = new Guid("5D29C300-39E8-4561-BABA-29207842176C");

        private static Guid _Setting_UatAuthUrl = new Guid("8E35946F-A6F6-561C-B4DF-CDCF5DD9BF0C");

        private static Guid _Setting_UatRequestUrl = new Guid("D13A6C1D-C087-427D-820F-4D3A797FC857");

        private static Guid _Setting_DebugMode = new Guid("B8BAD158-4245-4E85-B9ED-BDE09B2DE613");

		public string ClientId;

		public string ClientSecret;

		public string AuthURL;

		public string RequestURL;

        public string UatClientId;

        public string UatClientSecret;

        public string UatAuthURL;

        public string UatRequestURL;

        public bool DebugMode;
	}
}
