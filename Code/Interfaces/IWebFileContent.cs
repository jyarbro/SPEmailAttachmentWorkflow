using System.IO;

namespace OKM.EmailAttachmentWorkflow.Code.Interfaces {
	public interface IWebFileContent {
		string Name { get; set; }
		Stream Stream { get; set; }
	}
}