using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Dynamic
{
	/// <summary>
	/// Provides a dynamic model based on a dictionary of 
	/// key-value pairs.
	/// </summary>
	public class DictionaryModel : IReflectableType
	{
		/// <summary>
		/// Initializes the model with the given dictionary.
		/// </summary>
		public DictionaryModel (IDictionary<string, object> dictionary)
		{
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
