using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using OKM.EmailAttachmentWorkflow.Code.Helpers;
using OKM.EmailAttachmentWorkflow.Code.Services;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace OKM.EmailAttachmentWorkflow.Features.OKM_Email_Attachment_Workflow_Activity {
	[Guid("555df13c-f96f-4119-beee-f6a721b41dbb")]
	public class OKM_Email_Attachment_Workflow_ActivityEventReceiver : SPFeatureReceiver {
		public override void FeatureActivated(SPFeatureReceiverProperties properties) {
			var logger = new SimpleLogger();
			var spWebConfigHelper = new SPWebConfigHelper(logger);
			var workflowType = typeof(WorkflowActivity.WorkflowActivity);

			if (properties.Feature.Parent is SPWebApplication parent) {
				var webConfigModification = new SPWebConfigModification() {
					Name = $"authorizedType[@Assembly='{workflowType.Assembly.FullName}'][@Namespace='{workflowType.Namespace}'][@TypeName='*'][@Authorized='True']",           // Make sure that the name is a unique XPath selector for the element we are adding. This name is used for removing the element
					Value = $@"<authorizedType Assembly=""{workflowType.Assembly.FullName}"" Namespace=""{workflowType.Namespace}"" TypeName=""*"" Authorized=""True"" />",     // The XML to insert as child node, make sure that used names match the Name selector
					Owner = $"OKM{workflowType.Name}",                                                                                                                          // The owner of the web.config modification, useful for removing a group of modifications
					Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode,                                                                                 // We are going to add a new XML node to web.config
					Path = "configuration/System.Workflow.ComponentModel.WorkflowCompiler/authorizedTypes/targetFx",                                                            // The XPath to the location of the parent node in web.config
					Sequence = 0                                                                                                                                                // Sequence is important if there are multiple equal nodes that can't be identified with an XPath expression
				};

				spWebConfigHelper.AddModification(parent.Id, new Collection<SPWebConfigModification>() {
					webConfigModification
				});
			}
		}

		public override void FeatureDeactivating(SPFeatureReceiverProperties properties) {
			var logger = new SimpleLogger();
			var spWebConfigHelper = new SPWebConfigHelper(logger);
			var workflowType = typeof(WorkflowActivity.WorkflowActivity);

			if (properties.Feature.Parent is SPWebApplication parent)
				spWebConfigHelper.RemoveModifications(parent, $"OKM{workflowType.Name}");
		}
	}
}
