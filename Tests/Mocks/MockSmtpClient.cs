using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System.Net.Mail;

namespace Tests.Mocks {
	public class MockSmtpClient : ISmtpClient {
		ILogger Log { get; }

		public MockSmtpClient(
			ILogger log
		) {
			Log = log;
		}

		public void Send(MailMessage mailMessage) {
			Log.Info("Mail message sent.");
		}
	}
}