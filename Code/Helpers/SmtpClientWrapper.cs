using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System.Net;
using System.Net.Mail;

namespace OKM.EmailAttachmentWorkflow.Code.Helpers {
	public class SmtpClientWrapper : ISmtpClient {
		ILogger Log { get; }
		SmtpClient SmtpClient { get; }

		public SmtpClientWrapper(
			ILogger log,
			IMailServerContext mailServerContext
		) {
			Log = log;

			var smtpClient = new SmtpClient(mailServerContext.MailServerAddress) {
				Credentials = CredentialCache.DefaultNetworkCredentials
			};

			SmtpClient = smtpClient;
		}

		public void Send(MailMessage mailMessage) {
			Log.Info($"Enter {nameof(SPWebConfigHelper)}.{nameof(SmtpClientWrapper.Send)}");

			SmtpClient.Send(mailMessage);

			Log.Info($"Leave {nameof(SPWebConfigHelper)}.{nameof(SmtpClientWrapper.Send)}");
		}
	}
}