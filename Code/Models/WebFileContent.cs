using System.IO;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;

namespace OKM.EmailAttachmentWorkflow.Code.Models {
	public class WebFileContent : IWebFileContent {
		public string Name { get; set; }
		public Stream Stream { get; set; }
	}
}