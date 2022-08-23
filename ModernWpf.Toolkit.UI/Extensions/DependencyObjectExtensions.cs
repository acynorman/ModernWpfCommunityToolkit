using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace ModernWpf.Toolkit.UI.Extensions
{
    public static class DependencyObjectExtensions
    {
        private static Dictionary<Guid, (DependencyPropertyDescriptor Descriptor, EventHandler Handler)> descriptors = new Dictionary<Guid, (DependencyPropertyDescriptor Descriptor, EventHandler Handler)>();

        public static Guid RegisterPropertyChangedCallback(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, DependencyPropertyChangedCallback callback)
        {
            Guid token = Guid.NewGuid();
            var descriptor = DependencyPropertyDescriptor.FromProperty(dependencyProperty, dependencyObject.GetType());
            EventHandler handler = (s, e) =>
            {
                callback.Invoke(dependencyObject, dependencyProperty);
            };

            descriptors.Add(token, (descriptor, handler));

            descriptor.AddValueChanged(dependencyObject, handler);
            return token;
        }

        public static void UnregisterPropertyChangedCallback(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, Guid token)
        {
            if (descriptors.TryGetValue(token, out (DependencyPropertyDescriptor Descriptor, EventHandler Handler) value))
            {
                value.Descriptor.RemoveValueChanged(dependencyObject, value.Handler);
                descriptors.Remove(token);
            }
        }
    }
}
