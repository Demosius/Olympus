﻿using System.Windows;
using System.Windows.Controls;

namespace AionClock.Views.Utility;

public class EventFocusAttachment
{
    public static Control GetElementToFocus(Button button)
    {
        return (Control)button.GetValue(ElementToFocusProperty);
    }

    public static void SetElementToFocus(Button button, Control value)
    {
        button.SetValue(ElementToFocusProperty, value);
    }

    public static readonly DependencyProperty ElementToFocusProperty =
        DependencyProperty.RegisterAttached("ElementToFocus", typeof(Control),
            typeof(EventFocusAttachment), new UIPropertyMetadata(null, ElementToFocusPropertyChanged));

    public static void ElementToFocusPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Button button)
        {
            button.Click += (_, _) =>
            {
                var control = GetElementToFocus(button);
                if (control == null) return;

                control.Focus();

                if (control is not TextBox tb) return;

                tb.AppendText(((Button)sender).Content.ToString());
                tb.CaretIndex = tb.Text.Length;
            };
        }
    }
}