using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace OratorCommonUtilities
{
    //see http://graemehill.ca/specifying-expected-datacontext-type-in-wpf
    public class DataContextHelper
    {
        public static Type GetExpectedDataContextType(DependencyObject obj)
        {
            return (Type)obj.GetValue(ExpectedDataContextTypeProperty);
        }

        public static void SetExpectedDataContextType(DependencyObject obj, Type value)
        {
            obj.SetValue(ExpectedDataContextTypeProperty, value);
        }

        public static readonly DependencyProperty ExpectedDataContextTypeProperty =
          DependencyProperty.RegisterAttached(
            "ExpectedDataContextType",
            typeof(Type),
            typeof(DataContextHelper),
            new UIPropertyMetadata(null, OnExpectedDataContextTypeChanged)
          );

        private static void OnExpectedDataContextTypeChanged(object obj, DependencyPropertyChangedEventArgs args)
        {
            var element = obj as FrameworkElement;
            element.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(OnElementLoaded));
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs args)
        {
            var element = sender as FrameworkElement;
            element.RemoveHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(OnElementLoaded));

            // Compare the expected type to the actual type
            var actualDataContextType = element.DataContext == null ? null : element.DataContext.GetType();
            var expectedDataContextType = GetExpectedDataContextType(element);

            if (!expectedDataContextType.IsAssignableFrom(actualDataContextType))
            {
#if DEBUG
                // The types don't match and debug mode is on so notify the developer 
                string actual = actualDataContextType == null ? "<null>" : actualDataContextType.ToString();
                string expected = expectedDataContextType.ToString();
                //MessageBox.Show(String.Format("DataContext has type {0}. Expected {1}.", actual, expected));
                System.Diagnostics.Debug.Print("DataContext has type {0}. Expected {1}.", actual, expected);
#endif
            }

        }
    }
}
