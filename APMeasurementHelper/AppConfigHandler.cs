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
                    bool found = false;

                    foreach (string key in Enum.GetNames(typeof(APP_CONFIG_KEY)))
                    {
                        found = false;

                        foreach (string inKey in keys)
                        {
                            if(inKey == key)
                            {
                                found = true;
                                break;
                            }
                        }

                        if(found == false)
                        {
                            this._config.AppSettings.Settings.Add(key, "");
                        }
                    }
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

        public void Save(APP_CONFIG_KEY key, string value)
        {
            switch( key )
            {
                case APP_CONFIG_KEY.rmcComPort:
                    this._config.AppSettings.Settings["rmcComPort"].Value = value;
                    break;

                case APP_CONFIG_KEY.mtkComPort:
                    this._config.AppSettings.Settings["mtkComPort"].Value = value;
                    break;

                case APP_CONFIG_KEY.apxFileName:
                    this._config.AppSettings.Settings["apxFileName"].Value = value;
                    break;

                case APP_CONFIG_KEY.excelFileName:
                    this._config.AppSettings.Settings["excelFileName"].Value = value;
                    break;

                case APP_CONFIG_KEY.arduinoFileName:
                    this._config.AppSettings.Settings["arduinoFileName"].Value = value;
                    break;
            }

            this._config.Save(ConfigurationSaveMode.Modified);
        }

        public string Get(APP_CONFIG_KEY key)
        {
            string result = string.Empty;

            switch (key)
            {
                case APP_CONFIG_KEY.rmcComPort:
                    result = ConfigurationManager.AppSettings["rmcComPort"];
                    break;

                case APP_CONFIG_KEY.mtkComPort:
                    result = ConfigurationManager.AppSettings["mtkComPort"];
                    break;

                case APP_CONFIG_KEY.apxFileName:
                    result = ConfigurationManager.AppSettings["apxFileName"];
                    break;

                case APP_CONFIG_KEY.excelFileName:
                    result = ConfigurationManager.AppSettings["excelFileName"];
                    break;

                case APP_CONFIG_KEY.arduinoFileName:
                    result = ConfigurationManager.AppSettings["arduinoFileName"];
                    break;
            }

            return result;
        }

        public string ErrorMessage { get; private set; } = string.Empty;
        private Configuration _config;
    }
}
