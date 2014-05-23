using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using Visiblox.Charts;
using Visiblox.Charts.Primitives;

namespace Profiler.LogManager
{
    public class GraphbuilderRulerBehaviour : RulerBehaviour
    {
       public event EventHandler<SelectionChangedEventArgs> SelectionActiveChangedEvent;
       public event EventHandler SelectionRemoved;

       private RulerRectangle _rectangle = null;

       public override void PointerPressed(IBehaviourEventSource sender, PointerEventContext context)
       {
           if (context.PointerButton != PointerButton.Right && Keyboard.Modifiers == ModifierKeys.None)
           {
               base.StartMeasuring(sender, context);
               base.PointerPressed(sender, context);
           }
       }

       public override void PointerReleased(IBehaviourEventSource sender, PointerEventContext context)
       {
           if (context.PointerButton != PointerButton.Right && Keyboard.Modifiers == ModifierKeys.None)
           {
               base.PointerReleased(sender, context);

               if (_rectangle.Width == 0.0 && this.SelectionRemoved != null)
                   this.SelectionRemoved(this, new EventArgs());
           }
       }

       public override void DeInit()
       {
           _rectangle.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(_rectangle_IsVisibleChanged);
           base.DeInit();
       }   

       protected override void Init()
       {
           base.Init();

           foreach (UIElement ele in BehaviourCanvas.Children)
           {
               if (ele is RulerRectangle)
               {                  
                   _rectangle = ele as RulerRectangle;
                   _rectangle.IsVisibleChanged += new DependencyPropertyChangedEventHandler(_rectangle_IsVisibleChanged);
               }
           }
       }

       private void _rectangle_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
       {
           this.SelectionActiveChangedEvent(this, new SelectionChangedEventArgs((bool)e.NewValue));
       }     
    }

    public class SelectionChangedEventArgs : EventArgs
    {
        public bool SelectionActive { get; set; }

        public SelectionChangedEventArgs(bool active)
        {
            SelectionActive = active;
        }
    }
}
