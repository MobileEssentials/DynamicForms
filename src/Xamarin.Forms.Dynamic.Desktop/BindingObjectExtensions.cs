using System.Reflection;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;

namespace Xamarin.Forms
{
	/// <summary>
	/// Exposes the <see cref="LoadFromXaml"/> extension method.
	/// </summary>
	public static class BindingObjectExtensions
	{
		static Func<BindableObject, string, BindableObject> loadXaml;

		static BindingObjectExtensions()
		{
			// This is the current situation, where the LoadFromXaml is the only non-public static method.
			var genericMethod = typeof (Xamarin.Forms.Xaml.Extensions)
				.GetMethods (BindingFlags.Static | BindingFlags.NonPublic).FirstOrDefault ();

			// If we didn't find it, it may be because the extension method may be public now :)
			if (genericMethod == null)
				genericMethod = typeof (Xamarin.Forms.Xaml.Extensions)
				.GetMethods (BindingFlags.Static | BindingFlags.Public)
				.FirstOrDefault (m => m.GetParameters().Last().ParameterType == typeof(string));

			if (genericMethod == null){
				loadXaml = (view, xaml) => { throw new NotSupportedException("Xamarin.Forms implementation of XAML loading not found. Please update the Dynamic nuget package."); };
			}
			else {
				genericMethod = genericMethod.MakeGenericMethod(typeof(BindableObject));
				loadXaml = (view, xaml) => (BindableObject)genericMethod.Invoke (null, new object[] { view, xaml });
			}
		}

		/// <summary>
		/// Applies the given XAML to the view.
		/// </summary>
		public static TView LoadFromXaml<TView> (this TView view, string xaml) where TView : BindableObject
		{
			return (TView)loadXaml (view, xaml);
		}
	}
}
