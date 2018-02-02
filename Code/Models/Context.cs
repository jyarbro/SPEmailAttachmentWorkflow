using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System;

namespace OKM.EmailAttachmentWorkflow.Code.Models {
	public class Context : IWorkflowItemContext, IMailServerContext {
		public Guid SiteId { get; set; }
		public Guid WebId { get; set; }
		public Guid ListId { get; set; }
		public int ListItemId { get; set; }

		public string MailServerAddress { get; set; }
		public string MailFromAddress { get; set; }
	}
}