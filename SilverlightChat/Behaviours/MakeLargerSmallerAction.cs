using System;
using System.Windows.Interactivity;
using System.Windows;

namespace Behaviors
{
    public class MakeLargerSmallerAction : TargetedTriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            StoryBoardHelper.PlayControlAnimation(Target, Percent);
        }

        public double Percent
        {
            get { return (double)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        public static readonly DependencyProperty PercentProperty =
            DependencyProperty.Register("Percent", typeof(double), typeof(MakeLargerSmallerAction), new PropertyMetadata((double)1));


    }
}
