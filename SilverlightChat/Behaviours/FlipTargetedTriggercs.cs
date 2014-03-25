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
using System.ComponentModel;

namespace SilverlightChat.Behaviours
{
    public enum RotationDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }

    public class Flip : TargetedTriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty FrontElementNameProperty =
            DependencyProperty.Register("FrontElementName", typeof(string),
                                        typeof(Flip), new PropertyMetadata(null));

        [Category("Flip Properties")]
        public string FrontElementName
        {
            get
            {
                return (string)GetValue(FrontElementNameProperty);
            }
            set
            {
                SetValue(FrontElementNameProperty, value);
            }
        }

        public static readonly DependencyProperty BackElementNameProperty =
            DependencyProperty.Register("BackElementName", typeof(string),
                                        typeof(Flip), new PropertyMetadata(null));

        [Category("Flip Properties")]
        public string BackElementName
        {
            get
            {
                return (string)GetValue(BackElementNameProperty);
            }
            set
            {
                SetValue(BackElementNameProperty, value);
            }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(Duration),
                                        typeof(Flip), new PropertyMetadata(null));

        [Category("Animation Properties")]
        public Duration Duration
        {
            get
            {
                return (Duration)GetValue(DurationProperty);
            }
            set
            {
                SetValue(DurationProperty, value);
            }
        }

        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register("Rotation", typeof(RotationDirection),
                                        typeof(Flip), new PropertyMetadata(RotationDirection.LeftToRight));

        [Category("Animation Properties")]
        public RotationDirection Rotation
        {
            get
            {
                return (RotationDirection)GetValue(RotationProperty);
            }
            set
            {
                SetValue(RotationProperty, value);
            }
        }

        public static readonly DependencyProperty FrontStoryboardProperty =
            DependencyProperty.Register("FrontStoryboard", typeof(Storyboard), typeof(Flip), null);

        public Storyboard FrontStoryBoard
        {
            get
            {
                return (Storyboard)GetValue(FrontStoryboardProperty);
            }
        }
        public static readonly DependencyProperty BackStoryboardProperty =
            DependencyProperty.Register("BackStoryboard", typeof(Storyboard), typeof(Flip), null);

        public Storyboard BackStoryboard
        {
            get
            {
                return (Storyboard)GetValue(BackStoryboardProperty);
            }
        }

        private bool _forward = true;

        protected override void OnAttached()
        {
            base.OnAttached();
            FrameworkElement element = this.AssociatedObject as FrameworkElement;
            if (element != null) element.Loaded += TargetLoaded;
        }

        void TargetLoaded(object sender, RoutedEventArgs e)
        {
            PlaneProjection pp = Target.Projection as PlaneProjection;
            if (Target.Projection == null)
            {
                pp = new PlaneProjection { CenterOfRotationY = .51 };

                Target.RenderTransformOrigin = new Point(.5, .5);
                Target.Projection = pp;
            }

            Storyboard sbF = new Storyboard();
            Storyboard sbB = new Storyboard();

            UIElement f = null;
            UIElement b = null;

            f = Target.FindName(FrontElementName) as UIElement;
            if (f != null)
            {
                PlaneProjection ppFront = new PlaneProjection { CenterOfRotationY = .51 };
                f.Projection = ppFront;
                f.RenderTransformOrigin = new Point(.5, .5);
            }
            b = Target.FindName(BackElementName) as UIElement;
            if (b != null)
            {
                PlaneProjection ppBack = new PlaneProjection { CenterOfRotationY = .51, RotationY = 180.0 };
                b.Projection = ppBack;
                b.RenderTransformOrigin = new Point(.5, .5);
                b.Opacity = 0.0;
            }


            double to = 0.0;
            double from = 180.0;
            string property = "RotationY";

            switch (Rotation)
            {
                case RotationDirection.RightToLeft:
                    to = 180.0;
                    from = 0.0;
                    break;
                case RotationDirection.TopToBottom:
                    property = "RotationX";
                    break;
                case RotationDirection.BottomToTop:
                    to = 0.0;
                    from = 180.0;
                    property = "RotationX";
                    break;
            }

            sbF.Duration = Duration;
            sbB.Duration = Duration;

            sbF.Children.Add(CreateDoubleAnimation(pp, property, from, to, true));
            sbB.Children.Add(CreateDoubleAnimation(pp, property, to, from, true));

            sbF.Children.Add(CreateDoubleAnimation(f, "Opacity", 1.0, 0.0, false));
            sbB.Children.Add(CreateDoubleAnimation(f, "Opacity", 0.0, 1.0, false));

            sbF.Children.Add(CreateDoubleAnimation(b, "Opacity", 0.0, 1.0, false));
            sbB.Children.Add(CreateDoubleAnimation(b, "Opacity", 1.0, 0.0, false));

            SetValue(FrontStoryboardProperty, sbF);
            SetValue(BackStoryboardProperty, sbB);


        }

        protected override void Invoke(object parameter)
        {
            Storyboard sbF = GetValue(FrontStoryboardProperty) as Storyboard;
            Storyboard sbB = GetValue(BackStoryboardProperty) as Storyboard;

            if (_forward)
            {
                sbF.Begin();
                _forward = false;
            }
            else
            {
                sbB.Begin();
                _forward = true;
            }
        }

        private static DoubleAnimation CreateDoubleAnimation(DependencyObject element, string property, double from,
                                                             double to, bool addEasing)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.To = to;
            da.From = from;
            if (addEasing)
                da.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut, Power = 3 };


            Storyboard.SetTargetProperty(da, new PropertyPath(property));
            Storyboard.SetTarget(da, element);
            return da;
        }
    }
}