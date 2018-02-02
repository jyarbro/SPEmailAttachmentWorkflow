using System;
using Microsoft.SharePoint;

namespace OKM.EmailAttachmentWorkflow.Code.Interfaces {
	public interface ISPActionManager {
		void WebAction(Action<SPSite, SPWeb> callback);
		void ListAction(Action<SPSite, SPWeb, SPList> callback);
	}
}