using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ZTCK.Lib.APMeasurementHelper
{
    public class AppConfigHandler
    {
        public enum APP_CONFIG_KEY
        {
            rmcComPort = 0,
            mtkComPort,
            apxFileName,
            excelFileName,
            arduinoFileName,

        }

        public AppConfigHandler()
        {
            this._config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public bool MakeKeys()
        {
            bool result = false;

            try
            {
                if( this._config.AppSettings.Settings.Count > 0 )
                {
                    string[] keys = this._config.AppSettings.Settings.AllKeys;

                }
                else
                {
                    foreach (string key in Enum.GetNames(typeof(APP_CONFIG_KEY)))
                    {
                        this._config.AppSettings.Settings.Add(key, "");
                    }
                }

                this._config.Save(ConfigurationSaveMode.Modified);
                result = true;
            }
            catch(Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }

            return result;
        }

        public string ErrorMessage { get; private set; } = string.Empty;
        private Configuration _config;
    }
}
