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
    public abstract class ModelBase : NotifyPropertyChangedBase
    {
        #region Constructor

        protected ModelBase()
        {
        }

        #endregion // Constructor
    }
}