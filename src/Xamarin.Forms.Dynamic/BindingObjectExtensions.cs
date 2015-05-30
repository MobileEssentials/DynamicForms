namespace Xamarin.Forms
{
	/// <summary>
	/// Provides the <see cref="LoadFromXaml"/> extension method.
	/// </summary>
	public static class BindingObjectExtensions
	{
		/// <summary>
		/// Applies the given XAML to the view.
		/// </summary>
		public static TView LoadFromXaml<TView> (this TView view, string xaml) where TView : BindableObject
		{
			return view;
		}
	}
}
