using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClickOncePublisher
{
    class Publisher
    {
        private const string WHITE_SPACE = " ";

        public VersionInfo GetCurrentVersionInfo()
        {
            using (StreamReader sr = new StreamReader(System.IO.Path.Combine(Config.Instance.WorkingDir, Config.Instance.AssemblyInfoPath)))
            {
                string version = sr.ReadToEnd();

                string versionRegex = @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]";
                Match m = Regex.Match(version, versionRegex);
                version = m.Value;

                versionRegex = @"(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})";
                m = Regex.Match(version, versionRegex);
                version = m.Value;

                string[] versionSplit = version.Split('.');

                return new VersionInfo(int.Parse(versionSplit[0]), int.Parse(versionSplit[1]),
                    int.Parse(versionSplit[2]), int.Parse(versionSplit[3]));
            }
        }

        public void GetLatest()
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append("/c");

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat(@"""{0}""", Config.Instance.TFExePath);

            arguments.Append(WHITE_SPACE);
            arguments.Append("get /recursive /noprompt /version:T");

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", arguments.ToString())
            {
                WorkingDirectory = Config.Instance.WorkingDir,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            System.Diagnostics.Process proc = new System.Diagnostics.Process() { StartInfo = startInfo };
            proc.Start();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
                throw new PublisherException("GetLatest failed!");
        }

        public void CheckoutFiles()
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append("/c");

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat(@"""{0}""", Config.Instance.TFExePath);

            arguments.Append(WHITE_SPACE);
            arguments.Append("checkout");

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("{0}", Config.Instance.AssemblyInfoPath);

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", arguments.ToString())
            {
                WorkingDirectory = Config.Instance.WorkingDir,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            System.Diagnostics.Process proc = new System.Diagnostics.Process() { StartInfo = startInfo };
            proc.Start();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
                throw new PublisherException("Checkout failed!");
        }

        public VersionInfo GetRequestedVersionInfo(string major, string minor, string build, string revision)
        {
            if (string.IsNullOrWhiteSpace(major) || string.IsNullOrWhiteSpace(minor) ||
                    string.IsNullOrWhiteSpace(build) || string.IsNullOrWhiteSpace(revision))
            {
                throw new PublisherException("Version numbers not set expectedly!");
            }

            return new VersionInfo(int.Parse(major), int.Parse(minor),
                int.Parse(build), int.Parse(revision));
        }

        public void SetNextVersionInfo(VersionInfo version)
        {
            string orig = string.Empty;

            using (StreamReader sr = new StreamReader(System.IO.Path.Combine(Config.Instance.WorkingDir, Config.Instance.AssemblyInfoPath)))
            {
                orig = sr.ReadToEnd();

                string versionRegex = @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]";
                string versionFileRegex = @"\[assembly\: AssemblyFileVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]";

                string newVersionInfo = string.Format(@"[assembly: AssemblyVersion(""{0}.{1}.{2}.{3}"")]",
                    version.Major, version.Minor, version.Build, version.Revision);
                string newVersionFileInfo = string.Format(@"[assembly: AssemblyFileVersion(""{0}.{1}.{2}.{3}"")]",
                    version.Major, version.Minor, version.Build, version.Revision);

                orig = Regex.Replace(orig, versionRegex, newVersionInfo);
                orig = Regex.Replace(orig, versionFileRegex, newVersionFileInfo);
            }

            if (orig != string.Empty)
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(Config.Instance.WorkingDir, Config.Instance.AssemblyInfoPath)))
                    sw.Write(orig);
        }

        public void Publish(VersionInfo version)
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append(Config.Instance.CloseConsoleOnFinish ? "/k" : "/c");

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat(@"""{0}""", Config.Instance.MSBuildExePath);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("{0}", Config.Instance.ProjFilePath);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:PublishDir={0}", Config.Instance.PublishDir + "/");

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:GenerateManifests={0}", Config.Instance.GenerateManifests);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:ApplicationVersion={0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:Install={0}", Config.Instance.Install);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:InstallUrl={0}", Config.Instance.InstallUrl);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:SupportUrl={0}", Config.Instance.SupportUrl);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:UpdateEnabled={0}", Config.Instance.UpdateEnabled);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:UpdateMode={0}", Config.Instance.UpdateMode);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:UpdateInterval={0}", Config.Instance.UpdateInterval);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:UpdateIntervalUnits={0}", Config.Instance.UpdateIntervalUnits);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:UpdatePeriodically={0}", Config.Instance.UpdatePeriodically);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:PublisherName={0}", Config.Instance.PublisherName);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:ProductName={0}", Config.Instance.ProductName);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:BootstrapperEnabled={0}", Config.Instance.BootstrapperEnabled);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:IsWebBootstrapper={0}", Config.Instance.IsWebBootstrapper);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:UpdateRequired={0}", Config.Instance.UpdateRequired);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:TargetCulture={0}", Config.Instance.TargetCulture);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:MinimumRequiredVersion={0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/property:SignManifests={0}", Config.Instance.SignManifests);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("/target:{0}", "Clean;Publish");

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", arguments.ToString())
            {
                WorkingDirectory = Config.Instance.WorkingDir,
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            System.Diagnostics.Process proc = new System.Diagnostics.Process() { StartInfo = startInfo };
            proc.Start();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
                throw new PublisherException("Publish failed!");
        }

        public void ModifyWebPage(VersionInfo version)
        {
            string path = System.IO.Path.GetFullPath(System.IO.Path.Combine("Resources", "PublishPage.htm"));
            string orig = string.Empty;

            using (StreamReader sr = new StreamReader(path))
            {
                orig = sr.ReadToEnd();

                string versionRegex = @"\d{1,}\.\d{1,}\.\d{1,}\.\d{1,}";
                string newVersionInfo = string.Format(@"{0}.{1}.{2}.{3}",
                    version.Major, version.Minor, version.Build, version.Revision);

                orig = Regex.Replace(orig, versionRegex, newVersionInfo);
            }

            if (orig != string.Empty)
                using (StreamWriter sw = new StreamWriter(path))
                    sw.Write(orig);

            string destPath = System.IO.Path.Combine(Config.Instance.PublishDir, "PublishPage.htm");
            File.Copy(path, destPath, true);
        }

        public void CheckinFiles(VersionInfo version)
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append("/c");

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat(@"""{0}""", Config.Instance.TFExePath);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("checkin /noprompt /comment:{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("{0}", Config.Instance.AssemblyInfoPath);

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", arguments.ToString())
            {
                WorkingDirectory = Config.Instance.WorkingDir,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            System.Diagnostics.Process proc = new System.Diagnostics.Process() { StartInfo = startInfo };
            proc.Start();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
                throw new PublisherException("Checkin failed!");
        }

        public void UndoCheckoutFiles()
        {
            this.CheckoutFiles(); // garanti olsun diye önce bir checkoutla

            StringBuilder arguments = new StringBuilder();

            arguments.Append("/c");

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat(@"""{0}""", Config.Instance.TFExePath);

            arguments.Append(WHITE_SPACE);
            arguments.Append("undo /noprompt");

            arguments.Append(WHITE_SPACE);
            arguments.AppendFormat("{0}", Config.Instance.AssemblyInfoPath);

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", arguments.ToString())
            {
                WorkingDirectory = Config.Instance.WorkingDir,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            System.Diagnostics.Process proc = new System.Diagnostics.Process() { StartInfo = startInfo };
            proc.Start();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
                throw new PublisherException("UndoCheckout failed!");
        }
    }
}
