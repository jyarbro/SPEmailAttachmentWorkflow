using System.Collections;

namespace OKM.EmailAttachmentWorkflow.Code.Interfaces {
	public interface IAttachmentMailerService {
		void Send(string emailSubject, string emailBody, ArrayList emailTo, ArrayList emailCC = null);
	}
}