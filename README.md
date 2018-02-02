# SPEmailAttachmentWorkflow
Allows you to email list item attachments and documents to an email recipient.

It's a farm solution, so you'll need Central Administration to install it.

Essentially you just need to install the package on your farm, and create a workflow on any list or doc library.

Select the workflow activity provided under the Office of Knowledge Management category. Configure the email to pull the recipient address from a metadata field or other workflow variable. Optionally, do something with the workflow result such as logging it to a workflow history list.

Supported documents can come from list item attachments (multiple attachments are supported), documents in a document library, document set or folder. Selecting a document set or folder will email all documents in that set or folder.
