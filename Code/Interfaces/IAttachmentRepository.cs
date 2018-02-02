using System.Collections.Generic;
using System.Net.Mail;

namespace OKM.EmailAttachmentWorkflow.Code.Interfaces {
	public interface IAttachmentRepository {
		List<Attachment> GetAttachments();
	}
}