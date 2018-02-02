using Microsoft.SharePoint;
using OKM.EmailAttachmentWorkflow.Code.Helpers;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System;

namespace OKM.EmailAttachmentWorkflow.Code.Services {
	public class SPActionManager : ISPActionManager {
		ILogger Log { get; }
		IWorkflowItemContext WorkflowItemContext { get; }

		public SPActionManager(
			ILogger log,
			IWorkflowItemContext workflowItemContext
		) {
			Log = log;
			WorkflowItemContext = workflowItemContext;
		}

		public void WebAction(Action<SPSite, SPWeb> callback) {
			Log.Info($"Enter {nameof(SPActionManager)}.{nameof(SPActionManager.WebAction)}");

			callback.ThrowIfNull(nameof(callback));

			SPSite spSite;
			SPWeb spWeb;

			SPSecurity.RunWithElevatedPrivileges(delegate () {
				spSite = new SPSite(WorkflowItemContext.SiteId);
				spWeb = spSite.OpenWeb(WorkflowItemContext.WebId);

				callback(spSite, spWeb);

				spWeb.Dispose();
				spSite.Dispose();
			});

			Log.Info($"Leave {nameof(SPActionManager)}.{nameof(SPActionManager.WebAction)}");
		}

		public void ListAction(Action<SPSite, SPWeb, SPList> callback) {
			Log.Info($"Enter {nameof(SPActionManager)}.{nameof(SPActionManager.ListAction)}");

			callback.ThrowIfNull(nameof(callback));

			SPSite spSite;
			SPWeb spWeb;
			SPList spList;

			SPSecurity.RunWithElevatedPrivileges(delegate () {
				spSite = new SPSite(WorkflowItemContext.SiteId);
				spWeb = spSite.OpenWeb(WorkflowItemContext.WebId);
				spList = spWeb.Lists[WorkflowItemContext.ListId];

				callback(spSite, spWeb, spList);

				spWeb.Dispose();
				spSite.Dispose();
			});

			Log.Info($"Leave {nameof(SPActionManager)}.{nameof(SPActionManager.ListAction)}");
		}
	}
}