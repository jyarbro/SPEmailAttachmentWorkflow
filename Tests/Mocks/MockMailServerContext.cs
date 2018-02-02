using OKM.EmailAttachmentWorkflow.Code.Interfaces;

namespace Tests.Mocks {
	public class MockMailServerContext : IMailServerContext {
		public string MailServerAddress { get; set; } = "http://testserver.priv";
		public string MailFromAddress { get; set; } = "emailtest@testserver.priv";
	}
}