using System.Net.Mail;

namespace OKM.EmailAttachmentWorkflow.Code.Interfaces {
	public interface ISmtpClient {
		void Send(MailMessage mailMessage);
	}
}