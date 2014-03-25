using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Behaviors
{
    public static class StoryBoardHelper
    {
        public static void PlayControlAnimation(UIElement controlToAnimate, double factor)
        {
            Storyboard story = new Storyboard();

            //stretch horizontally
            DoubleAnimationUsingKeyFrames scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            scaleXAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            scaleXAnimation.KeyFrames.Add(CreateFrame(factor, 100));
            Storyboard.SetTarget(scaleXAnimation, controlToAnimate);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            story.Children.Add(scaleXAnimation);

            //stretch vertically
            DoubleAnimationUsingKeyFrames scaleYAnimation = new DoubleAnimationUsingKeyFrames();
            scaleYAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            scaleYAnimation.KeyFrames.Add(CreateFrame(factor, 100)); 
            Storyboard.SetTarget(scaleYAnimation, controlToAnimate);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            story.Children.Add(scaleYAnimation);

            if (!(controlToAnimate.RenderTransform is TransformGroup))
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform transform = new ScaleTransform();
                transform.ScaleX = 1;
                transform.ScaleY = 1;
                group.Children.Add(transform);
                controlToAnimate.RenderTransformOrigin = new Point(0.5, 0.5);
                controlToAnimate.RenderTransform = group;
            }
            story.Begin();


        }

        private static SplineDoubleKeyFrame CreateFrame(double value, double duration)
        {
            SplineDoubleKeyFrame frame = new SplineDoubleKeyFrame();
            frame.Value = value;
            frame.KeyTime = TimeSpan.FromMilliseconds(duration);
            return frame;
        }
    }
}
