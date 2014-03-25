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
using System.Windows.Interactivity;

namespace SilverlightChat.Behaviours
{

    public class SnappingSlider : TargetedTriggerAction<Slider>
	{
		public SnappingSlider()
		{

		}

		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.ValueChanged +=new System.Windows.RoutedPropertyChangedEventHandler<double>(AssociatedObject_ValueChanged);
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			AssociatedObject.ValueChanged -= AssociatedObject_ValueChanged;
		}

		private void AssociatedObject_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			//Let the slider be equal to the Maximum and Minimum values
			if ((e.NewValue != AssociatedObject.Maximum) && (e.NewValue != AssociatedObject.Minimum))
			{
				//Using the Minimum value as a starting point, only allow values that are 
				//   a multiple of the SmallChange value. If you want to simply round to 
				//   integers, set it so that: 
				//   SmallChange = 1
				double calcValue = Math.Floor((e.NewValue - AssociatedObject.Minimum) / AssociatedObject.SmallChange);
				calcValue = (calcValue * AssociatedObject.SmallChange) + AssociatedObject.Minimum;
				AssociatedObject.Value = Math.Round(calcValue);
			}
			else
			{
				//Sometimes it hiccups on me, so I used this to catch those
				AssociatedObject.Value = Math.Round(e.NewValue);
			}
		}

	}
}

