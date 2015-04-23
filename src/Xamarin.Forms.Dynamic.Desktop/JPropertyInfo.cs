using System;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.Dynamic
{
	internal class JPropertyInfo : PropertyInfo
	{
		JProperty json;

		public JPropertyInfo (JProperty json)
		{
			this.json = json;
		}

		public override PropertyAttributes Attributes
		{
			get { return PropertyAttributes.None; }
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override MethodInfo[] GetAccessors (bool nonPublic)
		{
			return new[] { GetGetMethod (nonPublic), GetSetMethod (nonPublic) };
		}

		public override MethodInfo GetGetMethod (bool nonPublic)
		{
			return new JPropertyGetterInfo (this, json);
		}

		public override ParameterInfo[] GetIndexParameters ()
		{
			return new ParameterInfo[0];
		}

		public override MethodInfo GetSetMethod (bool nonPublic)
		{
			return new JPropertySetterInfo (this, json);
		}

		public override object GetValue (object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException ();
		}

		public override Type PropertyType
		{
			get
			{
				switch (json.Value.Type) {
					case JTokenType.Integer:
						return typeof(int);
					case JTokenType.Float:
						return typeof(float);
					case JTokenType.String:
						return typeof(string);
					case JTokenType.Boolean:
						return typeof(bool);
					case JTokenType.Date:
						return typeof(DateTime);
					case JTokenType.Guid:
						return typeof(Guid);
					case JTokenType.TimeSpan:
						return typeof(TimeSpan);
					default:
						return typeof(object);
				}
			}
		}

		public override void SetValue (object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException ();
		}

		public override Type DeclaringType
		{
			get { throw new NotImplementedException (); }
		}

		public override object[] GetCustomAttributes (Type attributeType, bool inherit)
		{
			throw new NotImplementedException ();
		}

		public override object[] GetCustomAttributes (bool inherit)
		{
			throw new NotImplementedException ();
		}

		public override bool IsDefined (Type attributeType, bool inherit)
		{
			throw new NotImplementedException ();
		}

		public override string Name
		{
			get { throw new NotImplementedException (); }
		}

		public override Type ReflectedType
		{
			get { throw new NotImplementedException (); }
		}
	}
}
