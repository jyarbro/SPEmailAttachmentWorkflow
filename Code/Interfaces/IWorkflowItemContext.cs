using System;

namespace OKM.EmailAttachmentWorkflow.Code.Interfaces {
	public interface IWorkflowItemContext {
		Guid SiteId { get; set; }
		Guid WebId { get; set; }
		Guid ListId { get; set; }
		int ListItemId { get; set; }
	}
}