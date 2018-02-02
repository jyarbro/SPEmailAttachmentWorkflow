using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using OKM.EmailAttachmentWorkflow.Code.Services;
using System;
using System.Collections;
using System.Linq;
using Tests.Mocks;

namespace AttachmentMailerServiceTests {
	[TestClass]
	public class TestClass {
		ILifetimeScope ServiceLocator { get; set; }
		ILogger Log => ServiceLocator.Resolve<ILogger>();
		IAttachmentMailerService AttachmentMailerService => ServiceLocator.Resolve<IAttachmentMailerService>();

		[TestMethod]
		public void Run() {
			var toList = new ArrayList {
				"testemailto@testserver.priv",
				"testserver.priv\\user.name"
			};

			try {
				AttachmentMailerService.Send("Test subject", "Test emailBody", toList);
			}
			catch(Exception e) {
				Log.Exception(e);
			}

			Console.WriteLine(Log.BuildCallStack());
			Assert.IsFalse(Log.CallStack.Any());
		}

		[TestInitialize]
		public void Initialize() {
			var builder = new ContainerBuilder();

			builder.RegisterType<MockWorkflowItemContext>().As<IWorkflowItemContext>().InstancePerLifetimeScope();
			builder.RegisterType<MockMailServerContext>().As<IMailServerContext>().InstancePerLifetimeScope();
			builder.RegisterType<MockAttachmentRepository>().As<IAttachmentRepository>().InstancePerLifetimeScope();
			builder.RegisterType<MockSmtpClient>().As<ISmtpClient>().InstancePerLifetimeScope();
			builder.RegisterType<MockUserEmailRepository>().As<IUserEmailRepository>().InstancePerLifetimeScope();

			builder.RegisterType<SimpleLogger>().As<ILogger>().InstancePerLifetimeScope();
			builder.RegisterType<AttachmentMailerService>().As<IAttachmentMailerService>().InstancePerLifetimeScope();

			var container = builder.Build();
			ServiceLocator = container.BeginLifetimeScope();
		}

		[TestCleanup]
		public void Cleanup() {
			ServiceLocator.Dispose();
		}
	}
}