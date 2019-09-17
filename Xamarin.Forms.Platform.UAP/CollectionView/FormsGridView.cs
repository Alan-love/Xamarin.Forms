﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UWPApp = Windows.UI.Xaml.Application;
using UWPControls = Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	internal class FormsGridView : GridView, IEmptyView
	{
		int _span;
		ItemsWrapGrid _wrapGrid;
		ContentControl _emptyViewContentControl;
		FrameworkElement _emptyView;
		private Orientation _orientation;

		public FormsGridView()
		{
			RegisterPropertyChangedCallback(ItemsPanelProperty, ItemsPanelChanged);
			Loaded += OnLoaded;
		}

		public int Span
		{
			get => _span;
			set
			{
				_span = value;
				if (_wrapGrid != null)
				{
					UpdateItemSize();
				}
			}
		}

		public static readonly DependencyProperty EmptyViewVisibilityProperty =
			DependencyProperty.Register(nameof(EmptyViewVisibility), typeof(Visibility),
				typeof(FormsGridView), new PropertyMetadata(Visibility.Collapsed));

		public Visibility EmptyViewVisibility
		{
			get { return (Visibility)GetValue(EmptyViewVisibilityProperty); }
			set { SetValue(EmptyViewVisibilityProperty, value); }
		}

		public Orientation Orientation
		{
			get => _orientation;
			set
			{
				_orientation = value;
				if (_orientation == Orientation.Horizontal)
				{
					ItemsPanel = (ItemsPanelTemplate)UWPApp.Current.Resources["HorizontalGridItemsPanel"];
					ScrollViewer.SetHorizontalScrollMode(this, ScrollMode.Auto);
					ScrollViewer.SetHorizontalScrollBarVisibility(this, UWPControls.ScrollBarVisibility.Auto);
				}
				else
				{
					ItemsPanel = (ItemsPanelTemplate)UWPApp.Current.Resources["VerticalGridItemsPanel"];
				}
			}
		}

		void FindItemsWrapGrid()
		{
			_wrapGrid = this.GetFirstDescendant<ItemsWrapGrid>();

			if (_wrapGrid == null)
			{
				return;
			}

			_wrapGrid.SizeChanged -= WrapGridSizeChanged;
			_wrapGrid.SizeChanged += WrapGridSizeChanged;
		}

		private void WrapGridSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateItemSize();
		}

		void UpdateItemSize()
		{
			if (_orientation == Orientation.Horizontal)
			{
				_wrapGrid.ItemHeight = _wrapGrid.ActualHeight / Span;
			}
			else
			{
				_wrapGrid.ItemWidth = _wrapGrid.ActualWidth / Span;
			}
		}

		void ItemsPanelChanged(DependencyObject sender, DependencyProperty dp)
		{
			FindItemsWrapGrid();
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			FindItemsWrapGrid();
		}

		public void SetEmptyView(FrameworkElement emptyView)
		{
			_emptyView = emptyView;

			if (_emptyViewContentControl != null)
			{
				_emptyViewContentControl.Content = emptyView;
			}
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_emptyViewContentControl = GetTemplateChild("EmptyViewContentControl") as ContentControl;

			if (_emptyView != null)
			{
				_emptyViewContentControl.Content = _emptyView;
			}
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			GroupFooterItemTemplateContext.EnsureSelectionDisabled(element, item);
			base.PrepareContainerForItemOverride(element, item);
		}
	}
}