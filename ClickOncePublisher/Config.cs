using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickOncePublisher
{
    public sealed class Config
    {
        private static volatile Config instance;
        private static object syncRoot = new Object();

        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Config();
                    }
                }

                return instance;
            }
        }

        public string PublishDir { get; private set; }
        public string MSBuildExePath { get; private set; }
        public string TFExePath { get; private set; }
        public string WorkingDir { get; private set; }
        public string AssemblyInfoPath { get; private set; }
        public string ProjFilePath { get; private set; }
        public string GenerateManifests { get; private set; }
        public string Install { get; private set; }
        public string InstallUrl { get; private set; }
        public string SupportUrl { get; private set; }
        public string UpdateEnabled { get; private set; }
        public string UpdateMode { get; private set; }
        public string UpdateInterval { get; private set; }
        public string UpdateIntervalUnits { get; private set; }
        public string UpdatePeriodically { get; private set; }
        public string PublisherName { get; private set; }
        public string ProductName { get; private set; }
        public string BootstrapperEnabled { get; private set; }
        public string IsWebBootstrapper { get; private set; }
        public string UpdateRequired { get; private set; }
        public string TargetCulture { get; private set; }
        public string SignManifests { get; private set; }
        public bool CloseConsoleOnFinish { get; private set; }

        private Config() 
        {
            this.PublishDir = this.GetSetting("PublishDir");
            this.MSBuildExePath = this.GetSetting("MsBuildExePath");
            this.TFExePath = this.GetSetting("TfExePath");
            this.WorkingDir = this.GetSetting("WorkingDir");
            this.AssemblyInfoPath = this.GetSetting("AssemblyInfoPath");
            this.ProjFilePath = this.GetSetting("ProjFilePath");
            this.GenerateManifests = this.GetSetting("GenerateManifests");
            this.Install = this.GetSetting("Install");
            this.InstallUrl = this.GetSetting("InstallUrl");
            this.SupportUrl = this.GetSetting("SupportUrl");
            this.UpdateEnabled = this.GetSetting("UpdateEnabled");
            this.UpdateMode = this.GetSetting("UpdateMode");
            this.UpdateInterval = this.GetSetting("UpdateInterval");
            this.UpdateIntervalUnits = this.GetSetting("UpdateIntervalUnits");
            this.UpdatePeriodically = this.GetSetting("UpdatePeriodically");
            this.PublisherName = this.GetSetting("PublisherName");
            this.ProductName = this.GetSetting("ProductName");
            this.BootstrapperEnabled = this.GetSetting("BootstrapperEnabled");
            this.IsWebBootstrapper = this.GetSetting("IsWebBootstrapper");
            this.UpdateRequired = this.GetSetting("UpdateRequired");
            this.TargetCulture = this.GetSetting("TargetCulture");
            this.SignManifests = this.GetSetting("SignManifests");
            bool closeConsoleOnFinish = true;
            bool.TryParse(this.GetSetting("CloseConsoleOnFinish"), out closeConsoleOnFinish);
            this.CloseConsoleOnFinish = closeConsoleOnFinish;
        }

        private string GetSetting(string key)
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(key))
                throw new PublisherException("Setting not found in App.config. Key: {0}", key);

            return ConfigurationManager.AppSettings[key];
        }
    }
}
