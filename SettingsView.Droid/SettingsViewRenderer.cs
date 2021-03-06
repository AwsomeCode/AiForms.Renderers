﻿using System;
using AiForms.Renderers;
using AiForms.Renderers.Droid;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AListView = Android.Widget.ListView;
using Android.Content;

[assembly: ExportRenderer(typeof(SettingsView), typeof(SettingsViewRenderer))]
namespace AiForms.Renderers.Droid
{
    /// <summary>
    /// Settings view renderer.
    /// </summary>
    public class SettingsViewRenderer : ViewRenderer<SettingsView, AListView>
    {
        Page _parentPage;
        SettingsViewAdapter _adapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AiForms.Renderers.Droid.SettingsViewRenderer"/> class.
        /// </summary>
        public SettingsViewRenderer(Context context):base(context)
        {
            AutoPackage = false;
        }

        /// <summary>
        /// Ons the element changed.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<SettingsView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null) {
                var listview = new AListView(Context);
                SetNativeControl(listview);

                Control.Focusable = false;
                Control.DescendantFocusability = DescendantFocusability.AfterDescendants;
                Control.SetDrawSelectorOnTop(true);

                UpdateSelectedColor();
                UpdateBackgroundColor();
                UpdateRowHeight();

                _adapter = new SettingsViewAdapter(Context, e.NewElement, Control);
                Control.Adapter = _adapter;

                //hide Divider
                Control.DividerHeight = 0;
                Control.Divider = null;
                Control.SetFooterDividersEnabled(false);
                Control.SetHeaderDividersEnabled(false);

                Element elm = Element;
                while (elm != null) {
                    elm = elm.Parent;
                    if (elm is Page) {
                        break;
                    }
                }

                _parentPage = elm as Page;
                _parentPage.Appearing += ParentPageAppearing;
            }
        }

        void ParentPageAppearing(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => _adapter.DeselectRow());
        }

        /// <summary>
        /// Ons the element property changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == SettingsView.SeparatorColorProperty.PropertyName) {
                _adapter.NotifyDataSetChanged();
            }
            else if (e.PropertyName == SettingsView.BackgroundColorProperty.PropertyName) {
                UpdateBackgroundColor();
            }
            else if (e.PropertyName == TableView.RowHeightProperty.PropertyName) {
                UpdateRowHeight();
            }
            else if (e.PropertyName == SettingsView.UseDescriptionAsValueProperty.PropertyName) {
                _adapter.NotifyDataSetChanged();
            }
            else if (e.PropertyName == SettingsView.SelectedColorProperty.PropertyName) {
                UpdateSelectedColor();
            }
            else if (e.PropertyName == SettingsView.ShowSectionTopBottomBorderProperty.PropertyName) {
                _adapter.NotifyDataSetChanged();
            }
            else if (e.PropertyName == TableView.HasUnevenRowsProperty.PropertyName) {
                _adapter.NotifyDataSetChanged();
            }
            else if (e.PropertyName == SettingsView.ScrollToTopProperty.PropertyName){
                UpdateScrollToTop();
            }
            else if (e.PropertyName == SettingsView.ScrollToBottomProperty.PropertyName){
                UpdateScrollToBottom();
            }
        }

        void UpdateRowHeight()
        {
            if (Element.RowHeight == -1) {
                Element.RowHeight = 60;
            }
            else {
                _adapter?.NotifyDataSetChanged();
            }
        }

        void UpdateSelectedColor()
        {
            var color = Android.Graphics.Color.Rgb(180, 180, 180);
            if (Element.SelectedColor != Xamarin.Forms.Color.Default) {
                color = Element.SelectedColor.ToAndroid();
            }

            Control.Selector = DrawableUtility.CreateRipple(color);
        }

        void UpdateScrollToTop()
        {
            if (Element.ScrollToTop)
            {
                Control.SetSelection(0);
                Element.ScrollToTop = false;
            }
        }

        void UpdateScrollToBottom()
        {
            if (Element.ScrollToBottom)
            {
                var y = Control.GetChildAt(Control.ChildCount - 1).Top;
                Control.SetSelection(y);
                Element.ScrollToBottom = false;
            }
        }

        new void UpdateBackgroundColor()
        {
            if (Element.BackgroundColor != Xamarin.Forms.Color.Default) {
                Control.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
            }
        }

        /// <summary>
        /// Dispose the specified disposing.
        /// </summary>
        /// <returns>The dispose.</returns>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                _parentPage.Appearing -= ParentPageAppearing;
                Control?.Selector?.Dispose();
                _adapter?.Dispose();
                _adapter = null;
            }
            base.Dispose(disposing);
        }


    }


}
