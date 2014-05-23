using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;

namespace OratorCommonUtilities
{
    /// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications 
    /// and has a DisplayName property.  This class is abstract.
    /// </summary>
    public abstract class ViewModelBase : StatusUpdater
    {
        #region Constructor

        protected ViewModelBase()
        {
            this.AllowUserDelete = false;
        }

        #endregion // Constructor

        #region DisplayName

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public virtual string DisplayName { get; protected set; }

        #endregion // DisplayName

        #region AllowUserDelete
        /// <summary>
        /// Can the window be removed
        /// </summary>
        public virtual bool AllowUserDelete { get; set; }

        #endregion // AllowUserDelete

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        public ContentControl View { get; set; }
    }
}