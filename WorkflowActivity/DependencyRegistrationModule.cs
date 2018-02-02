using Autofac;
using OKM.EmailAttachmentWorkflow.Code.Helpers;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using OKM.EmailAttachmentWorkflow.Code.Services;

namespace OKM.EmailAttachmentWorkflow.WorkflowActivity {
	public class DependencyRegistrationModule : Module {
		protected override void Load(ContainerBuilder builder) {
			builder.RegisterType<AttachmentMailerService>().As<IAttachmentMailerService>().InstancePerLifetimeScope();
			builder.RegisterType<SimpleLogger>().As<ILogger>().InstancePerLifetimeScope();
			builder.RegisterType<SmtpClientWrapper>().As<ISmtpClient>().InstancePerLifetimeScope();
			builder.RegisterType<SPFileAttachmentRepository>().As<IAttachmentRepository>().InstancePerLifetimeScope();
			builder.RegisterType<SPActionManager>().As<ISPActionManager>().InstancePerLifetimeScope();
			builder.RegisterType<SPUserEmailRepository>().As<IUserEmailRepository>().InstancePerLifetimeScope();
		}
	}
}