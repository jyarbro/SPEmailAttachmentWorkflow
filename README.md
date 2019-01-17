# SPEmailAttachmentWorkflow
Allows you to email list item attachments and documents to an email recipient.

You will need to [sign the assemblies](https://docs.microsoft.com/en-us/dotnet/framework/app-domains/how-to-sign-an-assembly-with-a-strong-name) yourself, and build the `.wsp` package in Visual Studio on a machine that has SharePoint installed.

The three assemblies you will need to sign are from these projects:

- Code
- SharePoint
- WorkflowActivity

After you sign your assemblies, open [OKMEmailAttachmentWorkflow.actions](https://github.com/jyarbro/SPEmailAttachmentWorkflow/blob/master/SharePoint/Workflow/OKMEmailAttachmentWorkflow.actions) and update the public key token on line 6. You'll need to follow [this guide](https://blogs.msdn.microsoft.com/miah/2008/02/19/visual-studio-tip-get-public-key-token-for-a-strong-named-assembly/) if you aren't sure how to get the public key token. You'll need to find the public key token for the `key.snk` located in the WorkflowActivity project specifically, so make sure you highlight the right file before you run that `sn -T` tool.

Make sure you're targetting the correct assemblies for your version of SharePoint. I believe this code should work for SP2010 through 2016, as long as you update the `Microsoft.SharePoint` references.

It's a farm solution, so you'll need Central Administration to install it.

Essentially you just need to install the package on your farm, and create a workflow on any list or doc library.

Select the `Email a file` workflow activity provided under the `Office of Knowledge Management` category. Configure the email to pull the recipient address from a metadata field or other workflow variable. Optionally, do something with the workflow result such as logging it to a workflow history list.

Supported documents can come from list item attachments (multiple attachments are supported), documents in a document library, document set or folder. Selecting a document set or folder will email all documents in that set or folder.
