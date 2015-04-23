using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms
{
	public class JModel : JObject, IReflectableType
	{
		public JModel (params object[] content)
		{
		}

		public JModel (JObject other)
		{
		}

		/// <summary>
		/// Load a Newtonsoft.Json.Linq.JObject from a string that contains JSON.
		/// </summary>
		/// <param name="json">A System.String that contains JSON.</param>
		/// <returns>A Newtonsoft.Json.Linq.JObject populated from the string that contains JSON.</returns>
		public new static JModel Parse (string json)
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Loads an Newtonsoft.Json.Linq.JObject from a Newtonsoft.Json.JsonReader.
		/// </summary>
		/// <param name="reader">A Newtonsoft.Json.JsonReader that will be read for the content of the Newtonsoft.Json.Linq.JObject.</param>
		/// <returns>A Newtonsoft.Json.Linq.JObject that contains the JSON that was read from
		/// the specified Newtonsoft.Json.JsonReader.</returns>
		public new static JObject Load (JsonReader reader)
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Retrieves an object that represents the type of this instance.
		/// </summary>
		public TypeInfo GetTypeInfo ()
		{
			throw new NotImplementedException ();
		}
	}
}
