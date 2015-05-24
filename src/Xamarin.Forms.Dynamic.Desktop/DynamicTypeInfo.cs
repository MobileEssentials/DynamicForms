using System;
using System.Reflection;

namespace Xamarin.Forms.Dynamic
{
	internal class DynamicTypeInfo : TypeDelegator
	{
		Func<string, PropertyInfo> getProperty;

		public DynamicTypeInfo (Func<string, PropertyInfo> getProperty)
			: base (typeof (object))
		{
			this.getProperty = getProperty;
		}

		public override PropertyInfo GetDeclaredProperty (string name)
		{
			return getProperty (name);
		}
	}
}