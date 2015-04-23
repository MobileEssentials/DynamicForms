using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.Dynamic
{
	internal class JTypeInfo : TypeDelegator
	{
		JObject json;

		public JTypeInfo (JObject json)
			: base (typeof (JModel))
		{
			this.json = json;
		}

		public override PropertyInfo GetDeclaredProperty (string name)
		{
			var prop = json.Property (name);
			if (prop == null) {
				prop = new JProperty (name, "");
				json.Add (prop);
			}

			return new JPropertyInfo (prop);
		}
	}
}
