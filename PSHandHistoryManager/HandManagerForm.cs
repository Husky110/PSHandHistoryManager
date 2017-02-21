using System.Windows.Forms;
using System.Resources;
using System;
using System.Configuration;

namespace PSHandHistoryManager
{
    public class HandManagerForm : Form
    {
        protected ResourceManager resourceManager;
        protected HandManagerForm previousForm;

        protected void initializeHandManagerForm(HandManagerForm prevForm)
        {
            this.previousForm = prevForm;
        }
        protected void initializeHandManagerForm(Type childClassType)
        {
            this.resourceManager = new ResourceManager(childClassType.FullName, childClassType.Assembly);
        }
        protected void initializeHandManagerForm(HandManagerForm prevForm, Type childClassType)
        {
            this.initializeHandManagerForm(prevForm);
            this.initializeHandManagerForm(childClassType);
        }

        protected void showPreviousForm()
        {
            this.previousForm.Show();
            this.Close();
        }

        protected void setConfigValue(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection appsettings = (AppSettingsSection)config.GetSection("appSettings");
            appsettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
