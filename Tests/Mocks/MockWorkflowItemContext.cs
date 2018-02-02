using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System;

namespace Tests.Mocks {
	public class MockWorkflowItemContext : IWorkflowItemContext {
		public Guid SiteId { get; set; } = Guid.NewGuid();
		public Guid WebId { get; set; } = Guid.NewGuid();
		public Guid ListId { get; set; } = Guid.NewGuid();
		public int ListItemId { get; set; } = 47;
	}
}