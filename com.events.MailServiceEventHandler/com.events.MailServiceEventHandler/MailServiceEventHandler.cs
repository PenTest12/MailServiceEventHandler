using System;
using System.Web;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Tridion.ContentManager;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement.Fields;
using Tridion.ContentManager.Extensibility;
using Tridion.ContentManager.Extensibility.Events;
using Tridion.ContentManager.Publishing;
using Tridion.Logging;
using System.ServiceModel;
using System.Net.Mail;

namespace com.events.MailServiceEventHandler
{
    [TcmExtension("MailServiceEventHandlerEventSystemExtension")]
    class MailServiceEventHandler : TcmExtension
    {
        public MailServiceEventHandler()
        {
            Subscribe();
        }

        public void Subscribe()
        {

            EventSystem.Subscribe<Component, SaveEventArgs>(SentMailOnComponentPublish, EventPhases.Processed);
        }

        /// <summary>
        /// This method will update the meta data content of a component (Using Product schema).
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="args"></param>
        /// <param name="phase"></param>
        private void SentMailOnComponentPublish(Component component, EventArgs args, EventPhases phase)
        {

            string schemaURI = component.Schema.Title.ToString();
            Logger.Write(string.Format("SchemaURI-->  {0} ", schemaURI), "ComponentServiceEventHandlerEventSystemExtension", LoggingCategory.General, TraceEventType.Information);

            string schemaName = Utility.GetSchemafromConfig();
            Logger.Write(string.Format("SchemaName-->  {0} ", schemaName), "ComponentServiceEventHandlerEventSystemExtension", LoggingCategory.General, TraceEventType.Information);

            if (schemaURI.Contains(schemaName))
            {
                TcmUri componentID = component.Id;
                Logger.Write(string.Format("Component ID -->  {0} ", componentID), "ComponentServiceEventHandlerEventSystemExtension", LoggingCategory.General, TraceEventType.Information);

                string componentTitle = component.Title.ToString();
                Logger.Write(string.Format("Component Title -->  {0} ", componentTitle), "ComponentServiceEventHandlerEventSystemExtension", LoggingCategory.General, TraceEventType.Information);

                SendMail();
            }

        }

        private void SendMail()
        {
            MailMessage msg = Utility.WorkflowMailMessageConfiguration();
            SmtpClient client = Utility.SMTPClientConfiguration();

            try
            {
                client.Send(msg);
                Logger.Write(string.Format("Mail Sent -->  {0} ", msg.Body.ToString()), "ComponentServiceEventHandlerEventSystemExtension", LoggingCategory.General, TraceEventType.Information);
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error -->  {0} ", ex.Message), "ComponentServiceEventHandlerEventSystemExtension", LoggingCategory.General, TraceEventType.Error);
            }
        }
    }
}
