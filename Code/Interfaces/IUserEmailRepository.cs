using System.Collections;
using System.Collections.Generic;

namespace OKM.EmailAttachmentWorkflow.Code.Interfaces {
	public interface IUserEmailRepository {
		List<string> GetEmailsForUsers(ArrayList sourceList);
		string GetEmailForUser(string userId);
		bool IsMailAddress(string address);
	}
}