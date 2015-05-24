using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Xamarin.Forms.Dynamic
{
	public class DictionaryModelTests
	{
		[Fact]
		public void when_setting_key_then_raises_property_changed ()
		{
			var model = new DictionaryModel();
			var changed = "";
			model.PropertyChanged += (sender, args) => changed = args.PropertyName;

			model["Foo"] = "Bar";

			Assert.Equal ("Foo", changed);
		}

		[Fact]
		public void when_setting_key_dynamic_then_raises_property_changed ()
		{
			var model = new DictionaryModel();
			var changed = "";
			model.PropertyChanged += (sender, args) => changed = args.PropertyName;

			dynamic data = model;
			data.Foo = "Bar";

			Assert.Equal ("Foo", changed);
		}

		[Fact]
		public void when_setting_nested_key_dynamic_then_raises_property_changed ()
		{
			var model = new DictionaryModel (new Dictionary<string, object> 
			{
				{ "Foo", new Dictionary<string, object> 
					{
						{ "Bar", "Baz" }
					}
				}
			});

			var changed = "";
			model.PropertyChanged += (sender, args) => changed = args.PropertyName;

			dynamic data = model;
			data.Foo.Bar = "Hello";

			Assert.Equal ("Foo", changed);
		}

		[Fact]
		public void when_setting_nested_key_then_raises_property_changed ()
		{
			var model = new DictionaryModel (new Dictionary<string, object> 
			{
				{ "Foo", new Dictionary<string, object> 
					{
						{ "Bar", "Baz" }
					}
				}
			});

			var changed = "";
			model.PropertyChanged += (sender, args) => changed = args.PropertyName;

			var foo = (IDictionary<string, object>)model["Foo"];
			foo["Bar"] = "Hello";

			Assert.Equal ("Foo", changed);
		}
	}
}
