using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Xamarin.Forms.Dynamic
{
	public class JsonTests
	{
		[Fact]
		public void when_applying_changes_then_overwrites_properties ()
		{
			var source = JObject.Parse (@"{
	""Name"": ""Xamarin"",
	""Address"": {
		""Street"": ""Camila"",
		""Phone"": {
			""Prefix"": ""+54"",
			""Area"": ""11""
		}
	}
}");

			var update = JObject.Parse (@"{
	""Name"": ""Argentina"",
	""Address"": {
		""Street"": ""O'Gorman"",
		""Phone"": {
			""Area"": 3442
		}
	}
}");

			source.ApplyChanges (update);

			Assert.Equal ("Argentina", source.Property ("Name").Value.Value<string> ());
			Assert.Equal ("O'Gorman", source.Property ("Address").Value.Value<JObject> ().Property ("Street").Value.Value<string> ());
			Assert.Equal ("+54", source
				.Property ("Address").Value.Value<JObject> ()
				.Property ("Phone").Value.Value<JObject> ()
				.Property ("Prefix").Value.Value<string> ());

			Assert.Equal (3442, source
				.Property ("Address").Value.Value<JObject> ()
				.Property ("Phone").Value.Value<JObject> ()
				.Property ("Area").Value.Value<int> ());
		}

		[Fact]
		public void when_no_can_execute_exists_then_can_execute_true ()
		{
			var command = new JsonCommand (
				JObject.Parse (@"{ ""Name"": ""kzu""  }"), 
				JObject.Parse (@"{ ""Name"": ""vga""  }"));

			Assert.True (command.CanExecute (null));
		}

		[Fact]
		public void when_command_executed_then_applies_changes ()
		{
			var target = JObject.Parse (@"{ ""Name"": ""kzu""  }");
			var command = new JsonCommand (
				JObject.Parse (@"{ ""Name"": ""vga""  }"), 
				target);

			command.Execute (null);

			Assert.Equal ("vga", target.Property ("Name").Value.Value<string> ());
		}


		[Fact]
		public void when_can_execute_is_property_name_then_retrieves_value_from_target()
		{
			var command = new JsonCommand (
				JObject.Parse (@"{ ""$CanExecute"": ""IsEnabled"", ""Name"": ""kzu""  }"), 
				JObject.Parse (@"{ ""IsEnabled"": true  }"));

			Assert.True (command.CanExecute (null));
		}

		[Fact]
		public void when_can_execute_is_property_path_then_retrieves_value_from_target()
		{
			var command = new JsonCommand (
				JObject.Parse (@"{ ""$CanExecute"": ""Connection.Status.IsEnabled"", ""Name"": ""kzu""  }"), 
				JObject.Parse (@"
{ 
	""Connection"": {
		""Status"": {
			""IsEnabled"": true
		}
	}
}"));

			Assert.True (command.CanExecute (null));
		}

		[Fact]
		public void when_can_execute_property_path_changes_then_raises_can_execute_changed()
		{
			var target = JObject.Parse (@"
{ 
	""Connection"": {
		""Status"": {
			""IsEnabled"": true
		}
	}
}");
			var command = new JsonCommand (
				JObject.Parse (@"{ ""$CanExecute"": ""Connection.Status.IsEnabled"", ""Name"": ""kzu""  }"), 
				target);

			var changed = false;
			command.CanExecuteChanged += (_, __) => changed = true;

			target.Property ("Connection").Value.Value<JObject> ()
				.Property ("Status").Value.Value<JObject> ()
				.Property ("IsEnabled").Value = false;

			Assert.False (command.CanExecute (null));
			Assert.True (changed);
		}

		[Fact]
		public void when_can_execute_changes_then_raises_can_execute_changed()
		{
			var json = JObject.Parse (@"{ ""$CanExecute"": false, ""Name"": ""kzu""  }");
			var command = new JsonCommand (json, 
				JObject.Parse (@"{ ""Name"": ""vga""  }"));

			var changed = false;
			command.CanExecuteChanged += (_, __) => changed = true;

			Assert.False (json.Property ("$CanExecute").Value.Value<bool> ());
			Assert.False (command.CanExecute (null));

			json.Property ("$CanExecute").Value = true;

			Assert.True (command.CanExecute (null));
			Assert.True (changed);
		}
	}
}
