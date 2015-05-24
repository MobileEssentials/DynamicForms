using System;
using System.Globalization;
using System.Reflection;

namespace Xamarin.Forms.Dynamic
{
	internal class DynamicPropertyInfo : PropertyInfo
	{
		Type owner;
		Type type;
		string name;
		Func<object, object> getter;
		Action<object, object> setter;

		public DynamicPropertyInfo (Type owner, string name, Type type,
			Func<object, object> getter,
			Action<object, object> setter)
		{
			this.owner = owner;
			this.name = name;
			this.type = type;
			this.getter = getter;
			this.setter = setter;
		}

		public override bool CanRead
		{
			get { return getter != null; }
		}

		public override bool CanWrite
		{
			get { return setter != null; }
		}

		public override Type PropertyType
		{
			get { return type; }
		}

		public override string Name
		{
			get { return name; }
		}

		public override object GetValue (object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
		{
			return getter (obj);
		}

		public override void SetValue (object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
		{
			setter (obj, value);
		}

		public override MethodInfo GetGetMethod (bool nonPublic)
		{
			return new GetterMethodInfo (this);
		}

		public override MethodInfo GetSetMethod (bool nonPublic)
		{
			return new SetterMethodInfo (this);
		}

		public override PropertyAttributes Attributes
		{
			get { return PropertyAttributes.None; }
		}

		public override MethodInfo[] GetAccessors (bool nonPublic)
		{
			return new[] { GetGetMethod (nonPublic), GetSetMethod (nonPublic) };
		}

		public override ParameterInfo[] GetIndexParameters ()
		{
			return new ParameterInfo[0];
		}

		public override Type DeclaringType
		{
			get { return owner; }
		}

		public override object[] GetCustomAttributes (Type attributeType, bool inherit)
		{
			return new object[0];
		}

		public override object[] GetCustomAttributes (bool inherit)
		{
			return new object[0];
		}

		public override bool IsDefined (Type attributeType, bool inherit)
		{
			return false;
		}

		public override Type ReflectedType
		{
			get { return owner; }
		}

		abstract class DynamicPropertyMethodInfo : MethodInfo
		{
			public DynamicPropertyMethodInfo (PropertyInfo property)
			{
				Property = property;
			}

			protected PropertyInfo Property { get; private set; }

			public override string Name
			{
				get { return Property.Name; }
			}

			public override MethodInfo GetBaseDefinition ()
			{
				return null;
			}

			public override ICustomAttributeProvider ReturnTypeCustomAttributes
			{
				get { return null; }
			}

			public override MethodAttributes Attributes
			{
				get { return MethodAttributes.Public; }
			}

			public override MethodImplAttributes GetMethodImplementationFlags ()
			{
				return MethodImplAttributes.IL;
			}

			public override RuntimeMethodHandle MethodHandle
			{
				get { throw new NotImplementedException (); }
			}

			public override Type DeclaringType
			{
				get { return Property.DeclaringType; }
			}

			public override Type ReflectedType
			{
				get { return Property.ReflectedType; }
			}

			public override object[] GetCustomAttributes (Type attributeType, bool inherit)
			{
				return new object[0];
			}

			public override object[] GetCustomAttributes (bool inherit)
			{
				return new object[0];
			}

			public override bool IsDefined (Type attributeType, bool inherit)
			{
				return false;
			}
		}

		class GetterMethodInfo : DynamicPropertyMethodInfo
		{
			public GetterMethodInfo (PropertyInfo property)
				: base (property)
			{
			}

			public override object Invoke (object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
			{
				return Property.GetValue (obj, invokeAttr, binder, parameters, culture);
			}

			public override ParameterInfo[] GetParameters ()
			{
				return new[] { new DynamicParameterInfo (Property, Property.PropertyType, "value") };
			}

			public override Type ReturnType
			{
				get { return Property.PropertyType; }
			}
		}

		class SetterMethodInfo : DynamicPropertyMethodInfo
		{
			public SetterMethodInfo (PropertyInfo property)
				: base (property)
			{
			}

			public override object Invoke (object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
			{
				Property.SetValue (obj, parameters[0]);
				return null;
			}

			public override ParameterInfo[] GetParameters ()
			{
				return new[] {
					new DynamicParameterInfo (Property, Property.DeclaringType, "this"),
					new DynamicParameterInfo (Property, Property.PropertyType, "value")
				};
			}

			public override Type ReturnType
			{
				get { return typeof (void); }
			}
		}
	}
}
