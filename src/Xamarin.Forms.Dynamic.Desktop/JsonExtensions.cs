using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.Dynamic
{
	static class JsonExtensions
	{
		/// <summary>
		/// Patches an existing source <see cref="JObject"/> with (potentially 
		/// partial) changes from another <see cref="JObject"/>.
		/// </summary>
		public static void ApplyChanges(this JObject source, JObject changes)
		{
			// Metadata properties are never applied.
			foreach (var changedProp in changes.Properties().Where(prop => !prop.Name.StartsWith("$"))) {
				var sourceProp = source.Property (changedProp.Name);

				// If source property doesn't exist, add the new one directly.
				if (sourceProp == null) {
					source.Add (changedProp);
					continue;
				}
				
				// If the type changed, or if it's not a JObject, just overwrite 
				// the entire existing property.
				if (sourceProp.Value.Type != changedProp.Value.Type || 
					sourceProp.Value.Type != JTokenType.Object) {
					sourceProp.Value = changedProp.Value;
					continue;
				}

				var sourceObj = (JObject)sourceProp.Value;
				var changeObj = (JObject)changedProp.Value;

				// Recurse.
				ApplyChanges (sourceObj, changeObj);
			}

		}
	}
}
