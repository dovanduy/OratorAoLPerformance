using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace OratorPluginBase
{
    public class PluginViewBase : UserControl, IDisposable
    {
        public string ViewDisplayName
        {
            get { return (string)GetValue(ViewDisplayNameProperty); }
            set { SetValue(ViewDisplayNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewDisplayName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewDisplayNameProperty =
            DependencyProperty.Register("ViewDisplayName", typeof(string), typeof(PluginViewBase), new PropertyMetadata(""));     

        public bool AllowUserDelete { get; set; }

        public PluginViewBase()
        {
            AllowUserDelete = false;
            ViewDisplayName = "PluginViewBase";
        }

        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public virtual void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }
        #endregion // IDisposable Members
    }
}
