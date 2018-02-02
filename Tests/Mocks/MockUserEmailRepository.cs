using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Tests.Mocks {
	public class MockUserEmailRepository : IUserEmailRepository {
		public List<string> GetEmailsForUsers(ArrayList sourceList) {
			return new List<string> {
				"test1@test.priv",
				"test2@test.priv",
				"test3@test.priv"
			};
		}

		public string GetEmailForUser(string userId) {
			return "test@test.priv";
		}

		public bool IsMailAddress(string address) {
			return true;
		}
	}
}