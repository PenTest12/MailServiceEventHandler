using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace com.events.MailServiceEventHandler
{
        static class Utility
        {
            public static SmtpClient SMTPClientConfiguration()
            {
                AppSettingsSection section = GetAppSettings();

                SmtpClient client = new SmtpClient();

                //Get variable from 
                string _MailServer = section.Settings["MailServer"].Value;
                string _CredentialsID = section.Settings["CredentialsID"].Value;
                string _CredentialsPassword = section.Settings["CredentialsPassword"].Value;

                //Set value to smtp client
                client.Host = _MailServer;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential(_CredentialsID, _CredentialsPassword);

                return client;
            }

            public static MailMessage WorkflowMailMessageConfiguration()
            {

                AppSettingsSection section = GetAppSettings();
                /// <summary>
                /// Get Mail Settings
                /// </summary>
                string _MessageBodyXslt = section.Settings["MessageBodyXslt"].Value;
                string _MessageSubject = section.Settings["MessageSubject"].Value;
                string _MailTo = section.Settings["MailTo"].Value;
                string _MailFrom = section.Settings["MailFrom"].Value;

                MailMessage msg = new MailMessage(_MailFrom, _MailTo);
                msg.Body = GetHtmlBody(_MessageBodyXslt);
                msg.IsBodyHtml = true;
                msg.Subject = _MessageSubject;

                return msg;
            }

            public static string GetSchemafromConfig()
            {

                AppSettingsSection section = Utility.GetAppSettings();
                string schemaName = section.Settings["Schema"].Value;

                return schemaName;
            }

            public static string GetHtmlBody(string messageBodyXslt)
            {
                string xmlInput = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                                <EmailTemplate>
                                <subject>Information from xyz</subject>
                                <displayName>abcd</displayName>
                                <Message1>
                                    Thanks you for registering to xyz.
                                </Message1>
                                <Copyright>Copyright xyz</Copyright>
                                </EmailTemplate>";

                string xslInput = System.IO.File.ReadAllText(messageBodyXslt);

                using (StringReader srt = new StringReader(xslInput)) // xslInput is a string that contains xsl
                using (StringReader sri = new StringReader(xmlInput)) // xmlInput is a string that contains xml
                {
                    using (XmlReader xrt = XmlReader.Create(srt))
                    using (XmlReader xri = XmlReader.Create(sri))
                    {
                        XslCompiledTransform xslt = new XslCompiledTransform();
                        xslt.Load(xrt);
                        using (StringWriter sw = new StringWriter())
                        using (XmlWriter xwo = XmlWriter.Create(sw, xslt.OutputSettings)) // use OutputSettings of xsl, so it can be output as HTML
                        {
                            xslt.Transform(xri, xwo);
                            return sw.ToString();
                        }
                    }
                }
            }

            private static AppSettingsSection GetAppSettings()
            {
                System.Configuration.ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = @"<%Tridion Home%>\config\com.events.MailServiceEventHandler.dll.config";

                System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                AppSettingsSection section = (AppSettingsSection)configuration.GetSection("appSettings");
                return section;
            }
        }
    
}
