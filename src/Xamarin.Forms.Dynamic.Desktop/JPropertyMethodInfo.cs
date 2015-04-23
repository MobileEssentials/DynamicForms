using System;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.Dynamic
{
	internal abstract class JPropertyMethodInfo : MethodInfo
	{
		public JPropertyMethodInfo (PropertyInfo property, JProperty json)
		{
			Property = property;
			Json = json;
		}

		protected PropertyInfo Property { get; private set; }

		protected JProperty Json { get; private set; }

		public override MethodInfo GetBaseDefinition ()
		{
			return null;
		}

		public override MethodAttributes Attributes
		{
			get { return MethodAttributes.Public; }
		}

		public override ParameterInfo[] GetParameters ()
		{
			return new ParameterInfo[0];
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get { throw new NotImplementedException (); }
		}

		public override Type DeclaringType
		{
			get { return typeof (JModel); }
		}

		public override object[] GetCustomAttributes (Type attributeType, bool inherit)
		{
			return new object[0];
		}

		public override object[] GetCustomAttributes (bool inherit)
		{
			return new object[0];
		}

		public override MethodImplAttributes GetMethodImplementationFlags ()
		{
			return MethodImplAttributes.IL;
		}

		public override bool IsDefined (Type attributeType, bool inherit)
		{
			return false;
		}

		public override string Name
		{
			get { return Json.Name; }
		}

		public override Type ReflectedType
		{
			get { return typeof (JModel); }
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get { throw new NotImplementedException (); }
		}
	}
}
