using Newtonsoft.Json.Linq;
using Xunit;
using System.Reflection;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Dynamic;
using System.Collections.Generic;

namespace UnitTests
{
	public class DictionaryModelSpec
	{
		static DictionaryModelSpec()
		{
            Device.PlatformServices = new MockPlatformServices();
        }

		[Fact]
		public void when_binding_then_updates_view ()
		{
			var view = new MockView ();
			var model = new DictionaryModel(new Dictionary<string, object>
			{
				{ "Title", "Bar" }
			});

			view.SetBinding (MockView.TextProperty, "Title");
			view.BindingContext = model;

			Assert.Equal ("Bar", view.Text);
		}

		[Fact]
		public void when_changing_model_then_updates_view ()
		{
			var view = new MockView ();
			var model = new DictionaryModel(new Dictionary<string, object>
			{
				{ "Title", "Bar" }
			});

			view.SetBinding (MockView.TextProperty, "Title");
			view.BindingContext = model;

			model["Title"] = "Foo";

			Assert.Equal ("Foo", view.Text);
		}

		[Fact]
		public void when_loading_xaml_then_sets_bound_data ()
		{

			// Get Xaml from somewhere (i.e. VS)
			var xaml = @"<ContentPage xmlns='http://xamarin.com/schemas/2014/forms'
			 xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml'>
	<Label Text='{Binding Title, Mode=TwoWay}' VerticalOptions='Center' HorizontalOptions='Center' />
</ContentPage> ";

			var model = new DictionaryModel(new Dictionary<string, object>
			{
				{ "Title", "Bar" }
			});

			// Create empty container page for the XAML
			//		TODO: ensure the root element in the Xaml is actually a ContentPage?
			var view = new ContentPage ();
			
			// Set binding context to dummy data
			view.BindingContext = model;

			// Load the Xaml into the View 
			view.LoadFromXaml (xaml);

			// Grab the new label control added to the view via Xaml
			var label = view.Content as Label;
			Assert.NotNull (label);

			// UI properly bound to underlying model!
			Assert.Equal ("Bar", label.Text);

			// Change the label text via the control
			label.Text = "Foo";

			// Underlying model is updated!
			Assert.Equal ("Foo", model["Title"]);

            // Change the underlying model is
            model["Title"] = "Bar";

            // UI label properly reflecting underlying model change!
            Assert.Equal("Bar", label.Text);
        }

		[Fact]
		public void when_loading_xaml_then_sets_bound_data_to_complex_model ()
		{
			// Get Xaml from somewhere (i.e. VS)
			var xaml = @"<ContentPage xmlns='http://xamarin.com/schemas/2014/forms'
			 xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml'>
	<Label Text='{Binding Address.Phone.Prefix, Mode=TwoWay}' VerticalOptions='Center' HorizontalOptions='Center' />
</ContentPage> ";

			// Get dummy data from somewhere (i.e. VS)
			var model = new DictionaryModel(new Dictionary<string, object>
			{
				{ "Name", "Xamarin" },
				{ "Address", new Dictionary<string, object> 
					{
						{  "Street", "Camila" },
						{  "Phone", new Dictionary<string, object> 
							{
								{ "Prefix", "+54" },
								{ "Area", "11" },
							}
						},
					}
				}
			});

			// Create empty container page for the XAML
			//		TODO: ensure the root element in the Xaml is actually a ContentPage?
			var view = new ContentPage ();
			
			// Set binding context to dummy data
			view.BindingContext = model;

			// Load the Xaml into the View 
			//		TODO: this method is internal in XF.Xaml
			view.LoadFromXaml(xaml);

			// Grab the new label control added to the view via Xaml
			var label = view.Content as Label;
			Assert.NotNull (label);

			// UI properly bound to underlying model!
			Assert.Equal ("+54", label.Text);

			// Change the label text via the control
			label.Text = "+1";

			// Underlying model is updated!
			dynamic data = model;

			Assert.NotNull ((object)data.Address);
			Assert.NotNull ((object)data.Address.Phone);
			Assert.NotNull ((object)data.Address.Phone.Prefix);
			Assert.Equal ("+1", (string)data.Address.Phone.Prefix);

			// Change underlying data model
			data.Address.Phone.Prefix = "+54";

			Assert.Equal ("+54", label.Text);
		}

		public class MockView : BindableObject
		{
			public static readonly BindableProperty TextProperty = BindableProperty.Create<MockView, string> (x => x.Text, "Foo");

			public string Text
			{
				get { return (string)((string)base.GetValue (TextProperty)); }
				set { base.SetValue (TextProperty, value); }
			}
		}
	}
}
