using System;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.Dynamic
{
	internal class JTypeInfo : TypeInfo
	{
		TypeInfo type;
		JObject json;

		public JTypeInfo (JObject json)
		{
			type = typeof (JModel).GetTypeInfo ();
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

		public override Assembly Assembly
		{
			get { return type.Assembly; }
		}

		public override string AssemblyQualifiedName
		{
			get { return type.AssemblyQualifiedName; }
		}

		public override TypeAttributes Attributes
		{
			get { return type.Attributes; }
		}

		public override Type BaseType
		{
			get { return type.BaseType; }
		}

		public override bool ContainsGenericParameters
		{
			get { return type.ContainsGenericParameters; }
		}

		public override MethodBase DeclaringMethod
		{
			get { return type.DeclaringMethod; }
		}

		public override string FullName
		{
			get { return type.FullName; }
		}

		public override Guid GUID
		{
			get { return type.GUID; }
		}

		public override GenericParameterAttributes GenericParameterAttributes
		{
			get { return type.GenericParameterAttributes; }
		}

		public override int GenericParameterPosition
		{
			get { return type.GenericParameterPosition; }
		}

		public override Type[] GenericTypeArguments
		{
			get { return type.GenericTypeArguments; }
		}

		public override int GetArrayRank ()
		{
			return type.GetArrayRank ();
		}

		public override Type GetElementType ()
		{
			return type.GetElementType ();
		}

		public override Type[] GetGenericParameterConstraints ()
		{
			return type.GetGenericParameterConstraints ();
		}

		public override Type GetGenericTypeDefinition ()
		{
			return type.GetGenericTypeDefinition ();
		}

		public override bool IsEnum
		{
			get { return type.IsEnum; }
		}

		public override bool IsGenericParameter
		{
			get { return type.IsGenericParameter; }
		}

		public override bool IsGenericType
		{
			get { return type.IsGenericType; }
		}

		public override bool IsGenericTypeDefinition
		{
			get { return type.IsGenericTypeDefinition; }
		}

		public override bool IsSerializable
		{
			get { return type.IsSerializable; }
		}

		public override Type MakeArrayType (int rank)
		{
			return type.MakeArrayType (rank);
		}

		public override Type MakeArrayType ()
		{
			return type.MakeArrayType ();
		}

		public override Type MakeByRefType ()
		{
			return type.MakeByRefType ();
		}

		public override Type MakeGenericType (params Type[] typeArguments)
		{
			return type.MakeGenericType (typeArguments);
		}

		public override Type MakePointerType ()
		{
			return MakePointerType ();
		}

		public override string Namespace
		{
			get { return type.Namespace; }
		}

		public override Type DeclaringType
		{
			get { return type.DeclaringType; }
		}

		public override string Name
		{
			get { return type.Name; }
		}
	}
}
