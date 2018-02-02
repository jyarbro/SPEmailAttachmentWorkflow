using Autofac;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using OKM.EmailAttachmentWorkflow.Code.Services;
using System;
using System.Collections;
using System.Workflow.ComponentModel;

namespace OKM.EmailAttachmentWorkflow.WorkflowActivity {
	/*
	 * This class MUST exist in its own project, separate from any other projects, because System.Workflow.ComponentModel is incompatible with C# 6 compiler.
	 */

#pragma warning disable CS0618 // Disable obsolete warning, because the obsolete version is required due to lack of separate workflow server
	public class WorkflowActivity : Activity {
		public WorkflowContext __Context {
			get => ((WorkflowContext) (GetValue(__ContextProperty)));
			set => SetValue(__ContextProperty, value);
		}
		public static DependencyProperty __ContextProperty = DependencyProperty.Register(nameof(__Context), typeof(WorkflowContext), typeof(WorkflowActivity));

		public SPWorkflowActivationProperties __ActivationProperties {
			get => (SPWorkflowActivationProperties) GetValue(__ActivationPropertiesProperty);
			set => SetValue(__ActivationPropertiesProperty, value);
		}
		public static DependencyProperty __ActivationPropertiesProperty = DependencyProperty.Register(nameof(__ActivationProperties), typeof(SPWorkflowActivationProperties), typeof(WorkflowActivity));

		public string EmailSubject {
			get => (string) GetValue(EmailSubjectProperty);
			set => SetValue(EmailSubjectProperty, value);
		}
		public static DependencyProperty EmailSubjectProperty = DependencyProperty.Register(nameof(EmailSubject), typeof(string), typeof(WorkflowActivity));

		public string EmailBody {
			get => (string) GetValue(EmailBodyProperty);
			set => SetValue(EmailBodyProperty, value);
		}
		public static DependencyProperty EmailBodyProperty = DependencyProperty.Register(nameof(EmailBody), typeof(string), typeof(WorkflowActivity));

		public ArrayList EmailTo {
			get => (ArrayList) GetValue(EmailToProperty);
			set => SetValue(EmailToProperty, value);
		}
		public static DependencyProperty EmailToProperty = DependencyProperty.Register(nameof(EmailTo), typeof(ArrayList), typeof(WorkflowActivity));

		public ArrayList EmailCC {
			get => (ArrayList) GetValue(EmailCCProperty);
			set => SetValue(EmailCCProperty, value);
		}
		public static DependencyProperty EmailCCProperty = DependencyProperty.Register(nameof(EmailCC), typeof(ArrayList), typeof(WorkflowActivity));

		public string Result {
			get => (string) GetValue(ResultProperty);
			set => SetValue(ResultProperty, value);
		}
		public static DependencyProperty ResultProperty = DependencyProperty.Register(nameof(Result), typeof(string), typeof(WorkflowActivity));

		protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext) {
			Result = "Workflow started.";

			try {
				if (!SPUtility.IsEmailServerSet(__Context.Web))
					throw new Exception("Email server is not set on this web.");

				using (var serviceLocator = OpenServiceLocator()) {
					var attachmentMailerService = serviceLocator.Resolve<IAttachmentMailerService>();
					attachmentMailerService.Send(EmailSubject, EmailBody, EmailTo, EmailCC);

					var log = serviceLocator.Resolve<ILogger>();
					log.Info("Workflow completed successfully.");
					Result = log.BuildCallStack();
				}
			}
			catch (Exception e) {
				Result = e.ToString();
			}

			if (!string.IsNullOrEmpty(Result)) {
				var spWorkflowService = executionContext.GetService<ISharePointService>();
				var resultLines = Result.Split('\n');

				foreach (var line in resultLines) {
					spWorkflowService.LogToHistoryList(executionContext.ContextGuid,
						SPWorkflowHistoryEventType.WorkflowComment,
						0,
						TimeSpan.Zero,
						"Information",
						line,
						string.Empty);
				}
			}

			return ActivityExecutionStatus.Closed;
		}

		protected ILifetimeScope OpenServiceLocator() {
			var context = new ContextFactory().Create(__Context);

			var builder = new ContainerBuilder();
			builder.RegisterModule<DependencyRegistrationModule>();
			builder.RegisterInstance(context).As<IMailServerContext>().As<IWorkflowItemContext>().SingleInstance();

			var container = builder.Build();

			return container.BeginLifetimeScope();
		}
	}
#pragma warning restore CS0618
}