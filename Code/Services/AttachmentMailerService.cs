using OKM.EmailAttachmentWorkflow.Code.Helpers;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System.Collections;
using System.Linq;
using System.Net.Mail;

namespace OKM.EmailAttachmentWorkflow.Code.Services {
	public class AttachmentMailerService : IAttachmentMailerService {
		ILogger Log { get; }
		IMailServerContext MailServerContext { get; }
		ISmtpClient SmtpClient { get; }
		IAttachmentRepository AttachmentRepository { get; }
		IUserEmailRepository UserEmailRepository { get; }

		public AttachmentMailerService(
			ILogger log,
			IMailServerContext mailServerContext,
			ISmtpClient smtpClient,
			IAttachmentRepository attachmentRepository,
			IUserEmailRepository userEmailRepository
		) {
			Log = log;
			MailServerContext = mailServerContext;
			SmtpClient = smtpClient;
			AttachmentRepository = attachmentRepository;
			UserEmailRepository = userEmailRepository;
		}

		public void Send(string emailSubject, string emailBody, ArrayList emailTo, ArrayList emailCC = null) {
			Log.Info($"Enter {nameof(AttachmentMailerService)}.{nameof(AttachmentMailerService.Send)}");

			emailTo.ThrowIfNull(nameof(emailTo));

			var email = new MailMessage {
				From = new MailAddress(MailServerContext.MailFromAddress),
				Subject = emailSubject,
				Body = emailBody,
				IsBodyHtml = true,
			};

			var toAddresses = UserEmailRepository.GetEmailsForUsers(emailTo);

			if (!toAddresses.Any()) {
				Log.SerializeObjectToLog(emailTo);
				Log.Error("No email addresses found in collection.");
				return;
			}

			var ccAddresses = UserEmailRepository.GetEmailsForUsers(emailCC);
			var attachments = AttachmentRepository.GetAttachments();

			if (!attachments.Any()) {
				Log.Error("No files found in item.");
				return;
			}

			foreach (var item in toAddresses)
				email.To.Add(item);

			foreach (var item in ccAddresses)
				email.CC.Add(item);

			foreach (var attachment in attachments)
				email.Attachments.Add(attachment);

			SmtpClient.Send(email);

			Log.Info($"Leave {nameof(AttachmentMailerService)}.{nameof(AttachmentMailerService.Send)}");
		}
	}
}