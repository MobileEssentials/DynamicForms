using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.Dynamic;

namespace Xamarin.Forms
{
	/// <summary>
	/// Dynamic data-bindable JSON-based model.
	/// </summary>
	public class JsonModel : JObject, IReflectableType
	{
		static readonly FieldInfo propertiesField = typeof (JObject).GetField ("_properties", BindingFlags.NonPublic | BindingFlags.Instance);

		ConcurrentDictionary<string, PropertyInfo> infos = new ConcurrentDictionary<string, PropertyInfo> ();
		Dictionary<string, ICommand> commands = new Dictionary<string, ICommand> ();

		ObservableCollection<JToken> children;
		IList<JToken> baseChildren;

		/// <summary>
		/// Creates the JSON-based model from content 
		/// objects from Linq to Json content.
		/// </summary>
		public JsonModel (params object[] content)
		{
			Initialize ();
			Add (content);
		}

		/// <summary>
		/// Creates the JSON-based model from a loaded  
		/// Linq to Json object.
		/// </summary>
		public JsonModel (JObject other)
		{
			Initialize ();
			foreach (JToken token in (IEnumerable<JToken>)other) {
				Add (token);
			}
		}

		/// <summary>
		/// Load a model from a string that contains JSON.
		/// </summary>
		/// <param name="json">A string that contains JSON.</param>
		/// <returns>A <see cref="JsonModel"/> populated from the string that contains JSON.</returns>
		public new static JsonModel Parse (string json)
		{
			return new JsonModel (JObject.Parse (json));
		}

		/// <summary>
		/// Loads an model from a JSON reader.
		/// </summary>
		/// <param name="reader">A <see cref="JsonReader "/> that will be read for the content of the <see cref="JsonModel"/>.</param>
		/// <returns>A <see cref="JsonModel"/> that contains the JSON that was read from the specified <see cref="JsonReader"/>.</returns>
		public new static JsonModel Load (JsonReader reader)
		{
			return new JsonModel (JObject.Load (reader));
		}

		/// <summary>
		/// Retrieves an object that represents the type of this instance.
		/// </summary>
		public TypeInfo GetTypeInfo ()
		{
			// Populate commands first. They are immutable once schema is retrieved.
			var commands = Properties().FirstOrDefault(prop => 
				prop.Name.Equals("$commands", StringComparison.OrdinalIgnoreCase));

			if (commands != null) {
				if (commands.Value.Type != JTokenType.Object)
					throw new ArgumentException (string.Format ("The metadata property '{0}' must contain a JSON object value.", commands.Name));
				RefreshCommands ((JObject)commands.Value);
			}

			return new DynamicTypeInfo (name => infos.GetOrAdd (name, key => new DynamicPropertyInfo (
				  typeof (JsonModel),
				  key,
				  GetType (key),
				  obj => GetValue ((JsonModel)obj, key),
				  (obj, value) => SetValue ((JsonModel)obj, key, value))));
		}

		/// <summary>
		/// Gets the container's children tokens.
		/// </summary>
		protected override IList<JToken> ChildrenTokens
		{
			get { return children; }
		}

		void Initialize ()
		{
			children = new ObservableCollection<JToken> ();
			baseChildren = (IList<JToken>)propertiesField.GetValue (this);
			children.CollectionChanged += new NotifyCollectionChangedEventHandler (OnChanged);
		}

		void RefreshCommands (JObject commands)
		{
			this.commands = new Dictionary<string,ICommand>();

			commands.PropertyChanged -= OnCommandsChanged;
			commands.PropertyChanged += OnCommandsChanged;

			foreach (var property in commands.Properties()) {
				if (property.Value.Type != JTokenType.Object)
					throw new ArgumentException (string.Format ("Command property '{0}' does not have a JSON object value.", property.Name));

				this.commands[property.Name] = new JsonCommand (property.Value.Value<JObject> (), this);
			}
		}

		void OnCommandsChanged (object sender, PropertyChangedEventArgs args)
		{
			RefreshCommands ((JObject)sender);
		}

		void OnChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add) {
				var properties = e.NewItems
					.OfType<JProperty> ()
					.Where (prop => prop.Value.Type == JTokenType.Object &&
						!(prop.Value.Value<JObject> () is JsonModel));

				foreach (JProperty property in properties) {
					property.Value = new JsonModel (property.Value.Value<JObject> ());
				}

				foreach (JToken token in e.NewItems.OfType<JToken> ()) {
					baseChildren.Add (token);
				}

			} else if (e.Action == NotifyCollectionChangedAction.Remove) {
				foreach (JToken token in e.OldItems.OfType<JToken> ()) {
					baseChildren.Remove (token);
				}
			}
		}

		Type GetType (string key)
		{
			if (commands.ContainsKey (key))
				return typeof (ICommand);

			var prop = Property (key);
			if (prop == null)
				return typeof (object);

			switch (prop.Value.Type) {
				case JTokenType.Integer:
					return typeof (int);
				case JTokenType.Float:
					return typeof (float);
				case JTokenType.String:
					return typeof (string);
				case JTokenType.Boolean:
					return typeof (bool);
				case JTokenType.Date:
					return typeof (DateTime);
				case JTokenType.Guid:
					return typeof (Guid);
				case JTokenType.TimeSpan:
					return typeof (TimeSpan);
				default:
					return typeof (object);
			}
		}

		object GetValue (JsonModel model, string key)
		{
			ICommand command;
			if (commands.TryGetValue (key, out command))
				return command;

			var prop = model.Property (key);
			if (prop == null)
				return null;

			switch (prop.Value.Type) {
				case JTokenType.Integer:
					return prop.Value.Value<int> ();
				case JTokenType.Float:
					return prop.Value.Value<float> ();
				case JTokenType.String:
					return prop.Value.Value<string> ();
				case JTokenType.Boolean:
					return prop.Value.Value<bool> ();
				case JTokenType.Date:
					return prop.Value.Value<DateTime> ();
				case JTokenType.Guid:
					return prop.Value.Value<Guid> ();
				case JTokenType.TimeSpan:
					return prop.Value.Value<TimeSpan> ();
				case JTokenType.Object:
					return prop.Value.Value<JObject> ();
				default:
					return null;
			}
		}

		void SetValue (JsonModel model, string key, object value)
		{
			if (commands.ContainsKey (key))
				throw new InvalidOperationException ("Cannot set the value of a command property.");

			var prop = model.Property (key);
			if (prop == null) {
				prop = new JProperty (key, value);
				model.Add(prop);
				return;
			}
			
			switch (prop.Value.Type) {
				case JTokenType.Integer:
					prop.Value = Convert.ToInt32(value);
					break;
				case JTokenType.Float:
					prop.Value = Convert.ToSingle (value);
					break;
				case JTokenType.String:
					prop.Value = Convert.ToString(value);
					break;
				case JTokenType.Boolean:
					prop.Value = Convert.ToBoolean(value);
					break;
				case JTokenType.Date:
					prop.Value = Convert.ToDateTime(value);
					break;
				case JTokenType.Guid:
					prop.Value = (Guid)value;
					break;
				case JTokenType.TimeSpan:
					prop.Value = (TimeSpan)value;
					break;
				default:
					break;
			}
		}
	}
}