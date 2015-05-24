using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;

namespace Xamarin.Forms.Dynamic
{
	/// <summary>
	/// Provides a dynamic model based on a properties of 
	/// key-value pairs.
	/// </summary>
	public class DictionaryModel : DynamicObject,
		ICollection<KeyValuePair<string, object>>, IDictionary<string, object>,
		INotifyCollectionChanged, INotifyPropertyChanged,
		IReflectableType
	{
		ConcurrentDictionary<string, PropertyInfo> infos = new ConcurrentDictionary<string, PropertyInfo>();
		readonly Dictionary<string, object> properties = new Dictionary<string, object>();

		/// <summary>Event raised when the collection changes.</summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };

		/// <summary>Event raised when a property on the collection changes.</summary>
		public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

		/// <summary>
		/// Initializes an instance of the class.
		/// </summary>
		public DictionaryModel ()
			: this (new Dictionary<string, object> ())
		{
		}

		/// <summary>
		/// Initializes the model with the properties to be used 
		/// as properties.
		/// </summary>
		public DictionaryModel (IDictionary<string, object> properties)
		{
			foreach (var item in properties) {
				Add (item.Key, item.Value);
			}
		}

		Type GetType (string key)
		{
			if (!properties.ContainsKey (key) ||
				properties[key] == null)
				return typeof (object);

			return properties[key].GetType ();
		}

		object GetValue (IDictionary<string, object> properties, string key)
		{
			object value;
			properties.TryGetValue (key, out value);
			return value;
		}

		void SetValue (IDictionary<string, object> properties, string key, object value)
		{
			properties[key] = value;
		}

		void AddWithNotification (string key, object value)
		{
			value = WrapNestedDictionary (key, value);
			properties.Add (key, value);

			CollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Add,
				new KeyValuePair<string, object> (key, value)));
			PropertyChanged (this, new PropertyChangedEventArgs ("Count"));
			PropertyChanged (this, new PropertyChangedEventArgs ("Keys"));
			PropertyChanged (this, new PropertyChangedEventArgs ("Values"));
			PropertyChanged (this, new PropertyChangedEventArgs (key));
		}

		bool RemoveWithNotification (string key)
		{
			object value;
			if (properties.TryGetValue (key, out value) && properties.Remove (key)) {
				CollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Remove,
					new KeyValuePair<string, object> (key, value)));
				PropertyChanged (this, new PropertyChangedEventArgs ("Count"));
				PropertyChanged (this, new PropertyChangedEventArgs ("Keys"));
				PropertyChanged (this, new PropertyChangedEventArgs ("Values"));
				PropertyChanged (this, new PropertyChangedEventArgs (key));

				return true;
			}

			return false;
		}

		void UpdateWithNotification (string key, object value)
		{
			object existing;
			if (properties.TryGetValue (key, out existing)) {
				value = WrapNestedDictionary (key, value);
				properties[key] = value;

				CollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Replace,
					new KeyValuePair<string, object> (key, value),
					new KeyValuePair<string, object> (key, existing)));
				PropertyChanged (this, new PropertyChangedEventArgs ("Values"));
				PropertyChanged (this, new PropertyChangedEventArgs (key));
			} else {
				AddWithNotification (key, value);
			}
		}

		// Nested dictionaries are wrapped in our own model 
		// again to propagate property changes upwards.
		object WrapNestedDictionary (string key, object value)
		{
			var childProps = value as IDictionary<string, object>;
			if (childProps != null) {
				var innerModel = new DictionaryModel (childProps);
				innerModel.CollectionChanged += (sender, args) => PropertyChanged (this, new PropertyChangedEventArgs (key));
				return innerModel;
			}

			return value;
		}

		#region DynamicObject

		/// <summary>
		/// Tries to retrieve the value of a property using dynamic syntax.
		/// </summary>
		public override bool TryGetMember (GetMemberBinder binder, out object result)
		{
			return properties.TryGetValue (binder.Name, out result);
		}

		/// <summary>
		/// Sets the value of a dictionary key using dynamic syntax.
		/// </summary>
		public override bool TrySetMember (SetMemberBinder binder, object value)
		{
			this[binder.Name] = value;
			return true;
		}

		#endregion

		#region IReflectableType

		/// <summary>
		/// Retrieves an object that represents the type of this instance.
		/// </summary>
		public TypeInfo GetTypeInfo ()
		{
			return new DynamicTypeInfo (name => infos.GetOrAdd (name, key => new DynamicPropertyInfo (
				  typeof (DictionaryModel),
				  key,
				  GetType (key),
				  obj => GetValue ((DictionaryModel)obj, key),
				  (obj, value) => SetValue ((DictionaryModel)obj, key, value))));
		}

		#endregion

		#region IDictionary<string,object> Members

		/// <summary>
		/// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value of the element to add.</param>
		public void Add (string key, object value)
		{
			AddWithNotification (key, value);
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
		/// <returns>
		/// true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.
		/// </returns>
		public bool ContainsKey (string key)
		{
			return properties.ContainsKey (key);
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
		public ICollection<string> Keys
		{
			get { return properties.Keys; }
		}

		/// <summary>
		/// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns>
		/// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </returns>
		public bool Remove (string key)
		{
			return RemoveWithNotification (key);
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
		/// <returns>
		/// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.
		/// </returns>
		public bool TryGetValue (string key, out object value)
		{
			return properties.TryGetValue (key, out value);
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
		public ICollection<object> Values
		{
			get { return properties.Values; }
		}

		/// <summary>
		/// Gets or sets the element with the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public object this[string key]
		{
			get { return properties[key]; }
			set { UpdateWithNotification (key, value); }
		}

		#endregion

		#region ICollection<KeyValuePair<string,object>> Members

		void ICollection<KeyValuePair<string, object>>.Add (KeyValuePair<string, object> item)
		{
			AddWithNotification (item.Key, item.Value);
		}

		void ICollection<KeyValuePair<string, object>>.Clear ()
		{
			((ICollection<KeyValuePair<string, object>>)properties).Clear ();

			CollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
			PropertyChanged (this, new PropertyChangedEventArgs ("Count"));
			PropertyChanged (this, new PropertyChangedEventArgs ("Keys"));
			PropertyChanged (this, new PropertyChangedEventArgs ("Values"));
		}

		bool ICollection<KeyValuePair<string, object>>.Contains (KeyValuePair<string, object> item)
		{
			return ((ICollection<KeyValuePair<string, object>>)properties).Contains (item);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo (KeyValuePair<string, object>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, object>>)properties).CopyTo (array, arrayIndex);
		}

		int ICollection<KeyValuePair<string, object>>.Count
		{
			get { return ((ICollection<KeyValuePair<string, object>>)properties).Count; }
		}

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly
		{
			get { return ((ICollection<KeyValuePair<string, object>>)properties).IsReadOnly; }
		}

		bool ICollection<KeyValuePair<string, object>>.Remove (KeyValuePair<string, object> item)
		{
			return RemoveWithNotification (item.Key);
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,object>> Members

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator ()
		{
			return ((ICollection<KeyValuePair<string, object>>)properties).GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((ICollection<KeyValuePair<string, object>>)properties).GetEnumerator ();
		}

		#endregion
	}
}
