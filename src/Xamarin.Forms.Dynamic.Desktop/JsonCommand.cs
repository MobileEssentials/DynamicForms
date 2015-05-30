using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.Dynamic
{
	internal class JsonCommand : ICommand
	{
		JObject target;
		JObject command;

		JProperty canExecute;
		JObject canExecuteContext;

		public JsonCommand (JObject command, JObject target)
		{
			this.command = command;
			this.target = target;

			canExecute = command.Properties().FirstOrDefault(prop => 
				prop.Name.Equals("$CanExecute", StringComparison.OrdinalIgnoreCase));

			if (canExecute != null) {
				if (canExecute.Value.Type == JTokenType.Boolean) {
					canExecuteContext = command;
				} else if (canExecute.Value.Type == JTokenType.String) {
					canExecute = Dereference(canExecute.Value.Value<string> ());
					canExecuteContext = canExecute.Parent as JObject;
				}
			}

			if (canExecuteContext != null)
				canExecuteContext.PropertyChanged += OnPropertyChanged;
		}

		public bool CanExecute (object parameter)
		{
			return canExecute == null ? 
				true : 
				canExecute.Value.Type != JTokenType.Boolean ? 
				true : 
				canExecute.Value.Value<bool>();
		}

		public event EventHandler CanExecuteChanged = (sender, args) => { };

		public void Execute (object parameter)
		{
			target.ApplyChanges (command);
		}

		void OnPropertyChanged (object sender, PropertyChangedEventArgs args)
		{
			if (canExecute != null && args.PropertyName == canExecute.Name)
				CanExecuteChanged (this, EventArgs.Empty);
		}

		/// <summary>
		/// Dereferences a property name or path used as the $CanExecute property 
		/// value. Very useful to make the execution dynamic depending on a value 
		/// in the target model.
		/// </summary>
		JProperty Dereference (string reference)
		{
			var path = reference.Split ('.');
			var prop = default (JProperty);
			var token = (JToken)target;
			foreach (var name in path) {
				var context = token as JObject;
				if (context == null)
					break;

				prop = context.Property (name);
				if (prop == null)
					break;

				token = prop.Value;
			}

			return prop;
		}
	}
}
