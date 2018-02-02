using Microsoft.SharePoint;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace OKM.EmailAttachmentWorkflow.Code.Services {
	public class SPFileAttachmentRepository : IAttachmentRepository {
		ILogger Log { get; }
		IWorkflowItemContext WorkflowItemContext { get; }
		ISPActionManager ActionManager { get; }

		public SPFileAttachmentRepository(
			ILogger log,
			IWorkflowItemContext workflowItemContext,
			ISPActionManager actionManager
		) {
			Log = log;
			WorkflowItemContext = workflowItemContext;
			ActionManager = actionManager;
		}

		public List<Attachment> GetAttachments() {
			Log.Info($"Enter {nameof(SPFileAttachmentRepository)}.{nameof(SPFileAttachmentRepository.GetAttachments)}");

			var attachments = new List<Attachment>();

			ActionManager.ListAction((spSite, spWeb, spList) => {
				var spListItem = spList.GetItemById(WorkflowItemContext.ListItemId);

				if (spListItem.File != null) {
					var stream = GetStreamFromFile(spListItem.File);
					var attachment = new Attachment(stream, spListItem.File.Name);
					attachments.Add(attachment);
				}

				var attachmentCount = 0;

				try {
					attachmentCount = spListItem.Attachments.Count;
				}
				// If attachments doesn't exist
				catch (ArgumentException) { }

				if (attachmentCount > 0) {
					var spAttachmentCollection = spListItem.Attachments;

					foreach (string spFileUrl in spAttachmentCollection) {
						var spFile = spWeb.GetFile(spAttachmentCollection.UrlPrefix + spFileUrl);
						var stream = GetStreamFromFile(spFile);
						var attachment = new Attachment(stream, spFile.Name);
						attachments.Add(attachment);
					}
				}

				if (spListItem.Folder != null) {
					var fileCount = spListItem.Folder.Files.Count;

					if (fileCount > 0) {
						var spFileCollection = spListItem.Folder.Files;

						foreach (SPFile spFile in spFileCollection) {
							var stream = GetStreamFromFile(spFile);
							var attachment = new Attachment(stream, spFile.Name);
							attachments.Add(attachment);
						}
					}
				}
			});

			Log.Info($"Leave {nameof(SPFileAttachmentRepository)}.{nameof(SPFileAttachmentRepository.GetAttachments)}");

			return attachments;
		}

		public Stream GetStreamFromFile(SPFile spFile) {
			Log.Info($"Enter {nameof(SPFileAttachmentRepository)}.{nameof(SPFileAttachmentRepository.GetStreamFromFile)}");

			var memoryStream = new MemoryStream();

			using (var stream = spFile.OpenBinaryStream()) {
				stream.Position = 0;
				stream.CopyTo(memoryStream);
			}

			memoryStream.Position = 0;

			Log.Info($"Leave {nameof(SPFileAttachmentRepository)}.{nameof(SPFileAttachmentRepository.GetStreamFromFile)}");

			return memoryStream;
		}
	}
}