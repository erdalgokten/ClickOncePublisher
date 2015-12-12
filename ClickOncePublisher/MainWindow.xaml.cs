using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ClickOncePublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Publisher publisher = new Publisher();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            try
            {
                this.SetTextBoxes(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on filling text boxes! Reason: " + ex.Message, "Publisher Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void chkAutoIncrement_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.IsInitialized)
                    this.SetTextBoxes(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on filling text boxes! Reason: " + ex.Message, "Publisher Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void chkAutoIncrement_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.IsInitialized)
                    this.SetTextBoxes(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on filling text boxes! Reason: " + ex.Message, "Publisher Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetTextBoxes(bool autoIncrement)
        {
            var currentVersion = publisher.GetCurrentVersionInfo();

            this.txtMajor.Text = currentVersion.Major.ToString();
            this.txtMinor.Text = currentVersion.Minor.ToString();
            this.txtBuild.Text = currentVersion.Build.ToString();
            this.txtRevision.Text = (currentVersion.Revision + (autoIncrement ? 1 : 0)).ToString();

            this.txtMajor.IsEnabled =
            this.txtMinor.IsEnabled =
            this.txtBuild.IsEnabled =
            this.txtRevision.IsEnabled = !autoIncrement;
        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string txt = e.Text;
            int val = 0;

            if (!string.IsNullOrEmpty(txt))
                e.Handled = !int.TryParse(txt, out val);
        }

        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                publisher.GetLatest();
                publisher.CheckoutFiles();
                VersionInfo requestedVersion = publisher.GetRequestedVersionInfo(this.txtMajor.Text, this.txtMinor.Text, this.txtBuild.Text, this.txtRevision.Text); // user specified version
                publisher.SetNextVersionInfo(requestedVersion); // update assembly file
                publisher.Publish(requestedVersion);
                publisher.ModifyWebPage(requestedVersion); // modify web page and copy it to publish folder
                publisher.CheckinFiles(requestedVersion);

                MessageBox.Show("Publish succeeded!", "Publish Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Publish Failed! Reason: " + ex.Message, "Publish Result", MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
                    publisher.UndoCheckoutFiles();
                }
                catch (Exception undoEx)
                {
                    MessageBox.Show("Undo checkout failed! Reason: " + undoEx.Message, "Publisher Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
