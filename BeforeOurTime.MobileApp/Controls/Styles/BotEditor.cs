﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autofac;
using BeforeOurTime.MobileApp.Services.Loggers;
using BeforeOurTime.MobileApp.Services.Styles;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace BeforeOurTime.MobileApp.Controls
{
    /// <summary>
    /// Display a styled editor
    /// </summary>
    /// <example>
    /// </example>
    public class BotEditor : Editor
    {
        /// <summary>
        /// Dependency injection container
        /// </summary>
        public IContainer Services
        {
            get => (IContainer)GetValue(ServicesProperty);
            set => SetValue(ServicesProperty, value);
        }
        public static readonly BindableProperty ServicesProperty = BindableProperty.Create(
            nameof(Services), typeof(IContainer), typeof(BotEditor), null,
            propertyChanged: (BindableObject bindable, object oldvalue, object newvalue) =>
            {
                var control = (BotEditor)bindable;
                control.Services = (IContainer)newvalue;
                control.StyleService = control.Services.Resolve<IStyleService>();
                control.LoggerService = control.Services.Resolve<ILoggerService>();
                if (control.BotType != StyleType.Unknown)
                {
                    control.ApplyStyle(control.BotType);
                }
            });
        /// <summary>
        /// Style type
        /// </summary>
        public StyleType BotType
        {
            get => (StyleType)GetValue(BotTypeProperty);
            set => SetValue(BotTypeProperty, value);
        }
        public static readonly BindableProperty BotTypeProperty = BindableProperty.Create(
            nameof(BotType), typeof(StyleType), typeof(BotEditor), StyleType.Unknown,
            propertyChanged: (BindableObject bindable, object oldvalue, object newvalue) =>
            {
                var control = (BotEditor)bindable;
                control.BotType = (StyleType)newvalue;
                if (control.Services != null)
                {
                    control.ApplyStyle(control.BotType);
                }
            });
        /// <summary>
        /// Style service
        /// </summary>
        private IStyleService StyleService { set; get; }
        /// <summary>
        /// Logger service
        /// </summary>
        private ILoggerService LoggerService { set; get; }
        /// <summary>
        /// Constructor
        /// </summary>
        public BotEditor()
        {
        }
        /// <summary>
        /// Apply the style specified by the current template
        /// </summary>
        /// <param name="type"></param>
        public void ApplyStyle(StyleType type)
        {
            try
            {
                var editor = StyleService.GetTemplate()?.GetEditor(type);
                BackgroundColor = Color.FromHex(editor.BackgroundColor);
                TextColor = Color.FromHex(editor.TextColor);
                FontSize = editor.FontSize;
                Margin = editor.Margin;
            }
            catch (Exception e)
            {
                LoggerService.Log("Unable to apply style", e);
            }
        }
    }
}