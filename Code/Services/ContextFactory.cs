using Microsoft.SharePoint.WorkflowActions;
using OKM.EmailAttachmentWorkflow.Code.Models;
using System;

namespace OKM.EmailAttachmentWorkflow.Code.Services {
	public class ContextFactory {
		public Context Create(WorkflowContext workflowContext) {
			var listId = Guid.Parse(workflowContext.ListId);

			var context = new Context {
				SiteId = workflowContext.Site.ID,
				WebId = workflowContext.Web.ID,
				ListId = listId,
				ListItemId = workflowContext.ItemId,
				MailServerAddress = workflowContext.Site.WebApplication.OutboundMailServiceInstance.Server.Address,
				MailFromAddress = workflowContext.Site.WebApplication.OutboundMailSenderAddress,
			};

			return context;
		}
	}
}