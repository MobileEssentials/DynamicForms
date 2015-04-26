using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Xamarin.Forms;
using System.IO;

namespace DynamicFormsDemo
{
    public class App : Application
    {
        public App()
        {
			var content = new ContentPage ();
	
			var stream = this.GetType ().GetTypeInfo ().Assembly.GetManifestResourceStream ("DynamicFormsDemo.Main.xaml");
			var xaml = new StreamReader (stream).ReadToEnd ();
			content.LoadFromXaml (xaml);

			stream = this.GetType ().GetTypeInfo ().Assembly.GetManifestResourceStream ("DynamicFormsDemo.Main.json");
			var json = new StreamReader (stream).ReadToEnd ();
			var model = JModel.Parse (json);

			content.BindingContext = model;

			MainPage = content;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
