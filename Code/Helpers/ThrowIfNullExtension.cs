using System;
using System.Collections.Generic;
using System.Linq;

namespace OKM.EmailAttachmentWorkflow.Code.Helpers {
	public static class ThrowIfNullExtension {
		public static T ThrowIfNull<T>(this T argument, string argumentName) {
			if (argument == null)
				throw new ArgumentNullException(argumentName);

			if (argument is string) {
				var stringArgument = argument as string;

				if (string.IsNullOrEmpty(stringArgument))
					throw new ArgumentNullException(argumentName);
			}

			return argument;
		}

		public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> o, string name)
			where T : class {

			o.ThrowIfNull(name);

			if (!o.Any())
				throw new ArgumentException($"{name} is an empty enumerable.");
		}
	}
}