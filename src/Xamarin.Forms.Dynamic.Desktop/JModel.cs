using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.Dynamic;

namespace Xamarin.Forms
{
	/// <summary>
	/// Dynamic data-bindable JSON-based model.
	/// </summary>
	public class JModel : JObject, IReflectableType
	{
		static readonly FieldInfo propertiesField = typeof (JObject).GetField ("_properties", BindingFlags.NonPublic | BindingFlags.Instance);

		ObservableCollection<JToken> children;
		IList<JToken> baseChildren;

		/// <summary>
		/// Creates the JSON-based model from content 
		/// objects from Linq to Json content.
		/// </summary>
		public JModel (params object[] content)
		{
			Initialize ();
			Add (content);
		}

		/// <summary>
		/// Creates the JSON-based model from a loaded  
		/// Linq to Json object.
		/// </summary>
		public JModel (JObject other)
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
		/// <returns>A <see cref="JModel"/> populated from the string that contains JSON.</returns>
		public new static JModel Parse (string json)
		{
			return new JModel (JObject.Parse (json));
		}

		/// <summary>
		/// Loads an model from a JSON reader.
		/// </summary>
		/// <param name="reader">A <see cref="JsonReader "/> that will be read for the content of the <see cref="JModel"/>.</param>
		/// <returns>A <see cref="JModel"/> that contains the JSON that was read from the specified <see cref="JsonReader"/>.</returns>
		public new static JModel Load (JsonReader reader)
		{
			return new JModel (JObject.Load (reader));
		}

		/// <summary>
		/// Retrieves an object that represents the type of this instance.
		/// </summary>
		public TypeInfo GetTypeInfo ()
		{
			return new JTypeInfo (this);
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

		void OnChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add) {
				var properties = e.NewItems
					.OfType<JProperty> ()
					.Where (prop => prop.Value.Type == JTokenType.Object &&
						!(prop.Value.Value<JObject> () is JModel));

				foreach (JProperty property in properties) {
					property.Value = new JModel (property.Value.Value<JObject> ());
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
	}
}