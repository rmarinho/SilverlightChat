using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Behaviors
{
    public class SelectOnFocusAction : TargetedTriggerAction<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            Target.GotFocus += Target_GotFocus;
        }

        void Target_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            Target.SelectAll();
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            Target.GotFocus -= Target_GotFocus;
        }

        protected override void Invoke(object parameter)
        {
            //do nothing
            // we are handling specific event we are interested in
            // by attaching events to the target text
        }
        
    }
}
