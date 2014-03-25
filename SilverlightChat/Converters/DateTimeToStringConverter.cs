﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace SilverlightChat.Converters
{
       public class DateTimeToStringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType,
                object parameter, System.Globalization.CultureInfo culture)
            {
                // Retrieve the format string and use it to format the value.
                string formatString = parameter as string;
                if (!string.IsNullOrEmpty(formatString))
                {
                    return string.Format(culture, formatString, value);
                }

                // If the format string is null or empty, simply 
                // call ToString() on the value.
                return value.ToString();

            }

            public object ConvertBack(object value, Type targetType,
                object parameter, System.Globalization.CultureInfo culture)
            {
                // we don't intend this to ever be called
                return null;
            }
        }


    }

