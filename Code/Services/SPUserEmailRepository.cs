using OKM.EmailAttachmentWorkflow.Code.Helpers;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;

namespace OKM.EmailAttachmentWorkflow.Code.Services {
	public class SPUserEmailRepository : IUserEmailRepository {
		ILogger Log { get; }
		IWorkflowItemContext WorkflowItemContext { get; }
		ISPActionManager ActionManager { get; }

		public SPUserEmailRepository(
			ILogger log,
			IWorkflowItemContext workflowItemContext,
			ISPActionManager actionManager
		) {
			Log = log;
			WorkflowItemContext = workflowItemContext;
			ActionManager = actionManager;
		}

		public List<string> GetEmailsForUsers(ArrayList sourceList) {
			Log.Info($"Enter {nameof(SPUserEmailRepository)}.{nameof(SPUserEmailRepository.GetEmailsForUsers)}");

			var returnCollection = new List<string>();

			if (sourceList == null)
				return returnCollection;

			foreach (var item in sourceList) {
				if (item == null)
					continue;

				var value = item.ToString();

				if (!IsMailAddress(value))
					value = GetEmailForUser(value);

				if (!string.IsNullOrEmpty(value))
					returnCollection.Add(value);
			}

			Log.Info($"Leave {nameof(SPUserEmailRepository)}.{nameof(SPUserEmailRepository.GetEmailsForUsers)}");

			return returnCollection;
		}

		public string GetEmailForUser(string userId) {
			Log.Info($"Enter {nameof(SPUserEmailRepository)}.{nameof(SPUserEmailRepository.GetEmailForUser)}");

			userId.ThrowIfNull(nameof(userId));

			var emailAddress = string.Empty;

			ActionManager.WebAction((spSite, spWeb) => {
				var spUser = spWeb.EnsureUser(userId);

				if (spUser == null)
					return;

				emailAddress = spUser.Email;
			});

			Log.Info($"Leave {nameof(SPUserEmailRepository)}.{nameof(SPUserEmailRepository.GetEmailForUser)}");

			return emailAddress;
		}

		public bool IsMailAddress(string address) {
			Log.Info($"Enter {nameof(SPUserEmailRepository)}.{nameof(SPUserEmailRepository.IsMailAddress)}");

			var isMailAddress = false;

			if (!string.IsNullOrEmpty(address)) {
				try {
					var mailAddress = new MailAddress(address);
					isMailAddress = mailAddress != null;
				}
				catch (FormatException) { }
			}

			Log.Info($"Leave {nameof(SPUserEmailRepository)}.{nameof(SPUserEmailRepository.IsMailAddress)}");

			return isMailAddress;
		}
	}
}