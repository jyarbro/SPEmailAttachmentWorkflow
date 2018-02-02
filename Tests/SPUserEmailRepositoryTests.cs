using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using OKM.EmailAttachmentWorkflow.Code.Services;
using Tests.Mocks;

namespace SPUserEmailRepositoryTests {
	[TestClass]
	public class TestClass {
		ILifetimeScope ServiceLocator { get; set; }
		IUserEmailRepository UserEmailRepository => ServiceLocator.Resolve<IUserEmailRepository>();

		[TestMethod]
		public void IsMailAddress() {
			Assert.IsFalse(UserEmailRepository.IsMailAddress(null));
			Assert.IsFalse(UserEmailRepository.IsMailAddress(string.Empty));
			Assert.IsFalse(UserEmailRepository.IsMailAddress("test"));
			Assert.IsTrue(UserEmailRepository.IsMailAddress("test@test.priv"));
		}

		[TestInitialize]
		public void Initialize() {
			var builder = new ContainerBuilder();

			builder.RegisterType<MockWorkflowItemContext>().As<IWorkflowItemContext>().InstancePerLifetimeScope();
			builder.RegisterType<MockSPActionManager>().As<ISPActionManager>().InstancePerLifetimeScope();

			builder.RegisterType<SPUserEmailRepository>().As<IUserEmailRepository>().InstancePerLifetimeScope();
			builder.RegisterType<SimpleLogger>().As<ILogger>().InstancePerLifetimeScope();

			var container = builder.Build();
			ServiceLocator = container.BeginLifetimeScope();
		}

		[TestCleanup]
		public void Cleanup() {
			ServiceLocator.Dispose();
		}
	}
}
