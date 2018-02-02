using Microsoft.SharePoint.Administration;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace OKM.EmailAttachmentWorkflow.Code.Helpers {
	/// <summary>
	/// Helper class to add, clean, remove WebConfig modifications programmatically
	/// </summary>
	public class SPWebConfigHelper {
		ILogger Log { get; }

		public SPWebConfigHelper(
			ILogger log
		) {
			Log = log;
		}

		/// <summary>
		/// Method to add one or multiple WebConfig modifications
		/// NOTE: There should not be 2 modifications with the same owner.
		/// </summary>
		public void AddModification(Guid spWebAppId, Collection<SPWebConfigModification> modifications) {
			Log.Info($"Enter {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.AddModification)}");

			spWebAppId.ThrowIfNull(nameof(spWebAppId));
			modifications.ThrowIfNullOrEmpty(nameof(modifications));

			SPWebService.ContentService.ThrowIfNull(nameof(SPWebService.ContentService));

			var spWebApp = SPWebService.ContentService.WebApplications[spWebAppId];
			spWebApp.ThrowIfNull(nameof(spWebApp));

			var owners = modifications.Select(mod => mod.Owner).Distinct().ToList();
			RemoveModifications(spWebApp, owners);

			if (spWebApp.WebConfigModifications == null)
				throw new InvalidOperationException("Collection WebConfigModifications of webApplication is unexpectedly NULL. Cannot attempt to addition of web.config modification.");

			foreach (var modification in modifications)
				spWebApp.WebConfigModifications.Add(modification);

			spWebApp.Update();

			if (spWebApp.WebService == null)
				throw new InvalidOperationException($"Parent WebService of webApplication is unexpectedly NULL. Cannot execute {nameof(spWebApp.WebService.ApplyWebConfigModifications)}.");

			if (spWebApp.Farm == null)
				throw new InvalidOperationException($"Parent Farm of webApplication is unexpectedly NULL. Cannot execute {nameof(spWebApp.WebService.ApplyWebConfigModifications)}.");

			// Push modifications through the farm
			spWebApp.WebService.ApplyWebConfigModifications();

			// Wait for timer job
			try {
				WaitForWebConfigPropagation(spWebApp.Farm);
			}
			catch (Exception e) {
				Log.Error($"{nameof(SPWebConfigHelper)}: Failed to wait for propagation of addition of web.config modification to all servers in the Farm. Exception: {e.ToString()}");
			}

			Log.Info($"Leave {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.AddModification)}");
		}

		/// <summary>
		/// Method to remove all existing WebConfig Modifications by the same owner. By Design, owner should be unique so we can remove duplicates.
		/// NOTE: There should not be 2 modifications with the same owner.
		/// </summary>
		public void RemoveModifications(SPWebApplication spWebApp, string owner) => RemoveModifications(spWebApp, new List<string>() { owner });

		/// <summary>
		/// Method to remove all existing WebConfig Modifications for the listed owners.
		/// NOTE: There should not be 2 modifications with the same owner.
		/// </summary>
		public void RemoveModifications(SPWebApplication spWebApp, IList<string> owners) {
			Log.Info($"Enter {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.RemoveModifications)}");

			spWebApp.ThrowIfNull(nameof(spWebApp));

			var nullModificationIndexes = new List<int>();
			var modificationsToRemove = new Collection<SPWebConfigModification>();

			if (spWebApp.WebConfigModifications == null) {
				Log.Warn($"{nameof(SPWebConfigHelper)}: Attempted to remove web.config modification from web app with ID {spWebApp.Id} but no existing modification exists.");
				return;
			}

			var currentIndex = 0;

			foreach (var modification in spWebApp.WebConfigModifications) {
				if (modification != null) {
					if (!string.IsNullOrEmpty(modification.Owner)) {
						// collect modifications to delete
						if (owners.Contains(modification.Owner))
							modificationsToRemove.Add(modification);
					}
					else
						Log.Warn($"{nameof(SPWebConfigHelper)}: owner for existing modification {modification.Name} is empty. Cannot attempt removal.");
				}
				else {
					nullModificationIndexes.Add(currentIndex);
					Log.Warn($"{nameof(SPWebConfigHelper)}: web application with ID {spWebApp.Id} has a NULL modification.");
				}

				currentIndex++;
			}

			if (modificationsToRemove.Any() || nullModificationIndexes.Any()) {
				//clean up NULL values from web app's WebConfigModifications collection
				foreach (var index in nullModificationIndexes)
					spWebApp.WebConfigModifications.RemoveAt(index);

				// Remove the Owner's web config modification we want to clean up
				foreach (var modificationItem in modificationsToRemove)
					spWebApp.WebConfigModifications.Remove(modificationItem);

				// Commit modification removals to the specified web application
				spWebApp.Update();

				if (spWebApp.WebService == null)
					throw new InvalidOperationException($"Parent WebService of spWebApp is unexpectedly NULL. Cannot execute {nameof(spWebApp.WebService.ApplyWebConfigModifications)}.");

				if (spWebApp.Farm == null)
					throw new InvalidOperationException($"Parent Farm of spWebApp is unexpectedly NULL. Cannot execute {nameof(spWebApp.WebService.ApplyWebConfigModifications)}.");

				// Push modifications through the farm
				spWebApp.WebService.ApplyWebConfigModifications();

				// Wait for timer job 
				try {
					WaitForWebConfigPropagation(spWebApp.Farm);
				}
				catch (Exception e) {
					Log.Error($"{nameof(SPWebConfigHelper)}: Failed to wait for propagation of removal of web.config modification to all servers in the farm. Exception: {e.ToString()}");
				}
			}

			Log.Info($"Leave {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.RemoveModifications)}");
		}

		/// <summary>
		/// Waits for web configuration propagation. When there are multiple front-end Web servers in the SharePoint farm, we need to wait for the timer job that
		/// performs the Web.config modifications to complete before continuing. Otherwise, we may encounter the following error (e.g. when applying Web.config 
		/// changes from two different features in rapid succession): "A web configuration modification operation is already running."
		/// </summary>
		void WaitForWebConfigPropagation(SPFarm spFarm) {
			Log.Info($"Enter {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.WaitForWebConfigPropagation)}");

			if (spFarm.TimerService.Instances.Count > 1)
				WaitForOnetimeJobToFinish(spFarm, "Microsoft SharePoint Foundation Web.Config Update", 120);

			Log.Info($"Leave {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.WaitForWebConfigPropagation)}");
		}

		bool IsJobDefined(SPFarm spFarm, string jobTitle) {
			Log.Info($"Enter {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.IsJobDefined)}");

			var services = spFarm.Services;

			foreach (var service in services) {
				foreach (var job in service.JobDefinitions) {
					if (string.Compare(job.Title, jobTitle, StringComparison.OrdinalIgnoreCase) == 0)
						return true;
				}
			}

			Log.Info($"Leave {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.IsJobDefined)}");

			return false;
		}

		/// <summary>
		/// Determines whether the specified timer job is currently running (or scheduled to run).
		/// </summary>
		bool IsJobRunning(SPFarm spFarm, string jobTitle) {
			Log.Info($"Enter {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.IsJobRunning)}");

			var services = spFarm.Services;

			foreach (var service in services) {
				foreach (SPRunningJob job in service.RunningJobs) {
					if (string.Compare(job.JobDefinitionTitle, jobTitle, StringComparison.OrdinalIgnoreCase) == 0)
						return true;
				}
			}

			Log.Info($"Leave {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.IsJobRunning)}");

			return false;
		}

		/// <summary>
		/// Waits for a one-time SharePoint timer job to finish.
		/// </summary>
		/// <exception cref="ArgumentNullException">farm or jobTitle </exception>
		/// <exception cref="ArgumentException">The job title must be specified.;jobTitle</exception>
		void WaitForOnetimeJobToFinish(SPFarm spFarm, string jobTitle, int maximumWaitTimeInSeconds) {
			Log.Info($"Enter {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.WaitForOnetimeJobToFinish)}");

			spFarm.ThrowIfNull(nameof(spFarm));
			jobTitle.ThrowIfNull(nameof(jobTitle));

			var waitTime = 0F;

			do {
				var isJobDefined = IsJobDefined(spFarm, jobTitle);

				if (isJobDefined == false) {
					Console.WriteLine($"The timer job ({jobTitle}) is not defined. It may have been removed because the job completed.");
					break;
				}

				var isJobRunning = IsJobRunning(spFarm, jobTitle);

				Console.WriteLine($"The timer job '{jobTitle}' is currently {(isJobRunning ? "running" : "idle")}. Waiting for the job to finish...");

				var sleepTime = 5000; // milliseconds

				Thread.Sleep(sleepTime);
				waitTime += sleepTime / 1000.0F; // seconds
			}
			while (waitTime < maximumWaitTimeInSeconds);

			if (waitTime >= maximumWaitTimeInSeconds)
				Console.WriteLine($"Waited the maximum amount of time ({maximumWaitTimeInSeconds} seconds) for the one-time job ({jobTitle}) to finish.");
			else
				Console.WriteLine($"Waited {waitTime} seconds for the one-time job ({jobTitle}) to finish.");

			Log.Info($"Leave {nameof(SPWebConfigHelper)}.{nameof(SPWebConfigHelper.WaitForOnetimeJobToFinish)}");
		}
	}
}
