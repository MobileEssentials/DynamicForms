using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Xamarin.Forms.Dynamic
{
	public class CommandsTests
	{
		[Fact]
		public void when_mode_has_command_then_can_execute_it ()
		{
			var model = JsonModel.Parse (@"{
	""Name"": ""Xamarin"",
	""$commands"": {
		""SetNameCommand"": {
			""Name"": ""Rocks!""
		}
	}
}
");

			Assert.Equal ("Xamarin", model.Property ("Name").Value.Value<string> ());

			var info = model.GetTypeInfo ();
			Assert.NotNull (info);

			var prop = info.GetDeclaredProperty ("SetNameCommand");
			Assert.NotNull (prop);

			var command = (ICommand)prop.GetValue (model);
			command.Execute (null);
			
			Assert.Equal ("Rocks!", model.Property ("Name").Value.Value<string> ());
		}
	}
}
