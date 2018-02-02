using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace Tests.Mocks {
	public class MockAttachmentRepository : IAttachmentRepository {
		public List<Attachment> GetAttachments() {
			var memoryStream = new MemoryStream();

			using (var streamWriter = new StreamWriter(memoryStream)) {
				streamWriter.Write("Mock data");
				streamWriter.Write(Environment.NewLine);
				streamWriter.Write("More mock data");
				streamWriter.Flush();
			}

			var attachments = new List<Attachment> {
				new Attachment(memoryStream, "Mock file"),
				new Attachment(memoryStream, "Mock file 2")
			};

			return attachments;
		}
	}
}