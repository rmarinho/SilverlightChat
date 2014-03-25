using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightChat.Common
{

    //http://blogs.msdn.com/erwinvandervalk/archive/2009/10/12/how-to-work-with-animations-in-silverlight-in-the-mvvm-pattern.aspx
    public class VisualStateSetter
    {
        public static readonly DependencyProperty StateProperty =
        DependencyProperty.RegisterAttached("State", typeof(string), typeof(VisualStateSetter), new PropertyMetadata(StateChanged));
        public static void SetState(DependencyObject target, string state)
        {
            target.GetValue(StateProperty);
        }
        public static string GetState(DependencyObject target)
        {
            return target.GetValue(StateProperty) as string;
        }
        private static void StateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control host = d as Control;
            if (host == null)
                return;
            VisualStateManager.GoToState(host, e.NewValue as string, true);
        }
    }
}
