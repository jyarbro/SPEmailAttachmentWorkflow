namespace OKM.EmailAttachmentWorkflow.Code.Interfaces {
	public interface IMailServerContext {
		string MailServerAddress { get; set; }
		string MailFromAddress { get; set; }
	}
}