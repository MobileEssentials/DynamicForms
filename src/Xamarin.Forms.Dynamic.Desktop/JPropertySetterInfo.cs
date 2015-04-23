using System;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.Dynamic
{
	internal class JPropertySetterInfo : JPropertyMethodInfo
	{
		public JPropertySetterInfo (PropertyInfo property, JProperty json)
			: base (property, json)
		{
		}

		public override object Invoke (object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			switch (Json.Value.Type) {
				case JTokenType.Integer:
					Json.Value = (int)parameters[0];
					break;
				case JTokenType.Float:
					Json.Value = (float)parameters[0];
					break;
				case JTokenType.String:
					Json.Value = (string)parameters[0];
					break;
				case JTokenType.Boolean:
					Json.Value = (bool)parameters[0];
					break;
				case JTokenType.Date:
					Json.Value = (DateTime)parameters[0];
					break;
				case JTokenType.Guid:
					Json.Value = (Guid)parameters[0];
					break;
				case JTokenType.TimeSpan:
					Json.Value = (TimeSpan)parameters[0];
					break;
				default:
					break;
			}

			return null;
		}

		public override ParameterInfo[] GetParameters ()
		{
			return new[] {
				new JParameterInfo (Property, typeof(JModel), "this"),
				new JParameterInfo (Property, Property.PropertyType, "value")
			};
		}

		public override Type ReturnType
		{
			get { return typeof(void); }
		}
	}
}
