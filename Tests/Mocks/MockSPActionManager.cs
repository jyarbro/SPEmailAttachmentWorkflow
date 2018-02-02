using System;
using Microsoft.SharePoint;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;

namespace Tests.Mocks {
	public class MockSPActionManager : ISPActionManager {
		
		// These SPSite and SPWeb references shouldn't come from Microsoft.SharePoint namespace. They should be mockable wrappers instead.

		public void WebAction(Action<SPSite, SPWeb> callback) => throw new NotImplementedException();
		public void ListAction(Action<SPSite, SPWeb, SPList> callback) => throw new NotImplementedException();
	}
}