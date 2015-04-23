using System;
using System.Reflection;

namespace Xamarin.Forms.Dynamic
{
    internal class JParameterInfo : ParameterInfo
    {
		private MemberInfo member;
		Type type;

		public JParameterInfo (MemberInfo member, Type type, string name)
		{
			this.member = member;
			this.type = type;
		}

		public override MemberInfo Member
		{
			get { return member; }
		}

		public override Type ParameterType
		{
			get { return type; }
		}
	}
}
