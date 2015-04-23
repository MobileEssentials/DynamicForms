using System.Reflection;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	/// <summary>
	/// Provides the <see cref="LoadFromXaml"/> extension method.
	/// </summary>
	public static class BindingObjectExtensions
	{
		static readonly MethodInfo loadXaml = typeof (XamlCompilationAttribute)
			.GetMethods (BindingFlags.Static | BindingFlags.NonPublic).First ()
			.MakeGenericMethod (typeof (BindableObject));

		/// <summary>
		/// Applies the given XAML to the view.
		/// </summary>
		public static TView LoadFromXaml<TView> (this TView view, string xaml) where TView : BindableObject
		{
			// TODO: exposes Extensions.LoadFromXaml which is currently internal.
			loadXaml.Invoke (null, new object[] { view, xaml });

			return view;
		}
	}
}
