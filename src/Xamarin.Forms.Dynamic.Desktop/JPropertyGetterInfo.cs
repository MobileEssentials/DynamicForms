using System;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.Dynamic
{
	internal class JPropertyGetterInfo : JPropertyMethodInfo
	{
		public JPropertyGetterInfo (PropertyInfo property, JProperty json)
			: base (property, json)
		{
		}

		public override object Invoke (object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			switch (Json.Value.Type) {
				case JTokenType.Integer:
					return Json.Value.Value<int> ();
				case JTokenType.Float:
					return Json.Value.Value<float> ();
				case JTokenType.String:
					return Json.Value.Value<string> ();
				case JTokenType.Boolean:
					return Json.Value.Value<bool> ();
				case JTokenType.Date:
					return Json.Value.Value<DateTime> ();
				case JTokenType.Guid:
					return Json.Value.Value<Guid> ();
				case JTokenType.TimeSpan:
					return Json.Value.Value<TimeSpan> ();
				case JTokenType.Object:
					return Json.Value.Value<JObject> ();
				default:
					return null;
			}
		}

		public override ParameterInfo[] GetParameters ()
		{
			return new[] { new JParameterInfo (Property, Property.PropertyType, "value") };
		}

		public override Type ReturnType
		{
			get { return Property.PropertyType; }
		}
	}
}
