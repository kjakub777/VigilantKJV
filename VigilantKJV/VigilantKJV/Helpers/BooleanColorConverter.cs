using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using VigilantKJV.Views;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace VigilantKJV.Helpers
{
    public class BooleanColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color mem = Color.Transparent;
            if ((bool)value)
            {
                if (Application.Current.Resources.TryGetValue("Label_MemorizedHighlight", out object obj))
                    mem = (Color)obj;
            }
            else if (Application.Current.Resources.TryGetValue("LabelVerse_Background", out object obj))
                mem = (Color)obj;

            return mem;
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            // You probably don't need this, this is used to convert the other way around
            // so from color to yes no or maybe
            //   throw new NotImplementedException();
            return false;
        }
    }
    public class GetGradientColorConverter : IValueConverter
    {
        Xamarin.Forms.Color color1;
        public Xamarin.Forms.Color Color1
        {
            get
            {
                if (color1 == Color.Default)
                {
                    if (Application.Current.Resources.TryGetValue("LabelVerse_Background", out object obj))
                        color1 = (Color)obj;

                }
                return color1;
            }
        }
        Xamarin.Forms.Color color2;
        public Xamarin.Forms.Color Color2
        {
            get
            {
                if (color2 == Color.Default)
                {
                    if (Application.Current.Resources.TryGetValue("Label_MemorizedHighlight", out object obj))
                        color2 = (Color)obj;

                }
                return color2;
            }
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double percent = (double)value;

            double r = Color1.R + percent * (Color2.R - Color1.R);
            double g = Color1.G + percent * (Color2.G - Color1.G);
            double b = Color1.B + percent * (Color2.B - Color1.B);

            return Xamarin.Forms.Color.FromRgb(r, g, b);
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }

    }
}
