﻿﻿using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using WImageSource = Windows.UI.Xaml.Media.ImageSource;

namespace Xamarin.Forms.Platform.UWP
{
	public sealed class PageControl : ContentControl, IToolbarProvider
	{
		public static readonly DependencyProperty InvisibleBackButtonCollapsedProperty = DependencyProperty.Register("InvisibleBackButtonCollapsed", typeof(bool), typeof(PageControl),
			new PropertyMetadata(true, OnInvisibleBackButtonCollapsedChanged));

		public static readonly DependencyProperty ShowBackButtonProperty = DependencyProperty.Register("ShowBackButton", typeof(bool), typeof(PageControl),
			new PropertyMetadata(false, OnShowBackButtonChanged));

		public static readonly DependencyProperty TitleVisibilityProperty = DependencyProperty.Register(nameof(TitleVisibility), typeof(Visibility), typeof(PageControl), new PropertyMetadata(Visibility.Visible));

		public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register(nameof(ToolbarBackground), typeof(Brush), typeof(PageControl),
			new PropertyMetadata(default(Brush)));

		public static readonly DependencyProperty BackButtonTitleProperty = DependencyProperty.Register("BackButtonTitle", typeof(string), typeof(PageControl), new PropertyMetadata(false));

		public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register("ContentMargin", typeof(Windows.UI.Xaml.Thickness), typeof(PageControl),
			new PropertyMetadata(default(Windows.UI.Xaml.Thickness)));

		public static readonly DependencyProperty TitleIconProperty = DependencyProperty.Register(nameof(TitleIcon), typeof(WImageSource), typeof(PageControl), new PropertyMetadata(default(WImageSource)));

		public static readonly DependencyProperty TitleViewProperty = DependencyProperty.Register(nameof(TitleView), typeof(View), typeof(PageControl), new PropertyMetadata(default(View), OnTitleViewPropertyChanged));

		public static readonly DependencyProperty TitleViewVisibilityProperty = DependencyProperty.Register(nameof(TitleViewVisibility), typeof(Visibility), typeof(PageControl), new PropertyMetadata(Visibility.Collapsed));

		public static readonly DependencyProperty TitleInsetProperty = DependencyProperty.Register("TitleInset", typeof(double), typeof(PageControl), new PropertyMetadata(default(double)));

		public static readonly DependencyProperty TitleBrushProperty = DependencyProperty.Register("TitleBrush", typeof(Brush), typeof(PageControl), new PropertyMetadata(null));

		AppBarButton _backButton;
		CommandBar _commandBar;
		FrameworkElement _titleViewPresenter;

        ToolbarPlacement _toolbarPlacement;
	    readonly ToolbarPlacementHelper _toolbarPlacementHelper = new ToolbarPlacementHelper();

		public bool ShouldShowToolbar
		{
			get { return _toolbarPlacementHelper.ShouldShowToolBar; }
			set { _toolbarPlacementHelper.ShouldShowToolBar = value; }
		}

		public WImageSource TitleIcon
		{
			get { return (WImageSource)GetValue(TitleIconProperty); }
			set { SetValue(TitleIconProperty, value); }
		}

		public View TitleView
		{
			get { return (View)GetValue(TitleViewProperty); }
			set { SetValue(TitleViewProperty, value); }
		}

		TaskCompletionSource<CommandBar> _commandBarTcs;
		Windows.UI.Xaml.Controls.ContentPresenter _presenter;
	    		
		public PageControl()
		{
			DefaultStyleKey = typeof(PageControl);
		}

		public string BackButtonTitle
		{
			get { return (string)GetValue(BackButtonTitleProperty); }
			set { SetValue(BackButtonTitleProperty, value); }
		}

		public double ContentHeight
		{
			get { return _presenter != null ? _presenter.ActualHeight : 0; }
		}

		public Windows.UI.Xaml.Thickness ContentMargin
		{
			get { return (Windows.UI.Xaml.Thickness)GetValue(ContentMarginProperty); }
			set { SetValue(ContentMarginProperty, value); }
		}

		public double ContentWidth
		{
			get { return _presenter != null ? _presenter.ActualWidth : 0; }
		}

		public bool InvisibleBackButtonCollapsed
		{
			get { return (bool)GetValue(InvisibleBackButtonCollapsedProperty); }
			set { SetValue(InvisibleBackButtonCollapsedProperty, value); }
		}

		public Brush ToolbarBackground
		{
			get { return (Brush)GetValue(ToolbarBackgroundProperty); }
			set { SetValue(ToolbarBackgroundProperty, value); }
		}

        public ToolbarPlacement ToolbarPlacement
        {
            get { return _toolbarPlacement; }
            set
            {
                _toolbarPlacement = value; 
                _toolbarPlacementHelper.UpdateToolbarPlacement();
            }
        }

		public bool ShowBackButton
		{
			get { return (bool)GetValue(ShowBackButtonProperty); }
			set { SetValue(ShowBackButtonProperty, value); }
		}

		public Visibility TitleVisibility
		{
			get { return (Visibility)GetValue(TitleVisibilityProperty); }
			set { SetValue(TitleVisibilityProperty, value); }
		}

		public Visibility TitleViewVisibility
		{
			get { return (Visibility)GetValue(TitleViewVisibilityProperty); }
			set { SetValue(TitleViewVisibilityProperty, value); }
		}

		public Brush TitleBrush
		{
			get { return (Brush)GetValue(TitleBrushProperty); }
			set { SetValue(TitleBrushProperty, value); }
		}

		public double TitleInset
		{
			get { return (double)GetValue(TitleInsetProperty); }
			set { SetValue(TitleInsetProperty, value); }
		}

		Task<CommandBar> IToolbarProvider.GetCommandBarAsync()
		{
			if (_commandBar != null)
				return Task.FromResult(_commandBar);

			_commandBarTcs = new TaskCompletionSource<CommandBar>();
			ApplyTemplate();
			return _commandBarTcs.Task;
		}

		public event RoutedEventHandler BackClicked;

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_backButton = GetTemplateChild("backButton") as AppBarButton;
			if (_backButton != null)
				_backButton.Click += OnBackClicked;

			_presenter = GetTemplateChild("presenter") as Windows.UI.Xaml.Controls.ContentPresenter;

			_titleViewPresenter = GetTemplateChild("TitleViewPresenter") as FrameworkElement;

			_commandBar = GetTemplateChild("CommandBar") as CommandBar;


			_toolbarPlacementHelper.Initialize(_commandBar, () => ToolbarPlacement, GetTemplateChild);

			TaskCompletionSource<CommandBar> tcs = _commandBarTcs;
		    tcs?.SetResult(_commandBar);
		}

		void OnBackClicked(object sender, RoutedEventArgs e)
		{
			RoutedEventHandler clicked = BackClicked;
			if (clicked != null)
				clicked(this, e);
		}

		static void OnInvisibleBackButtonCollapsedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			((PageControl)dependencyObject).UpdateBackButton();
		}

		static void OnShowBackButtonChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			((PageControl)dependencyObject).UpdateBackButton();
		}

		static void OnTitleViewPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			((PageControl)dependencyObject).UpdateTitleViewPresenter();
		}

		void OnTitleViewPresenterLoaded(object sender, RoutedEventArgs e)
		{
			if (TitleView == null || _titleViewPresenter == null || _commandBar == null)
				return;

			_titleViewPresenter.Width = _commandBar.ActualWidth;
		}

		void UpdateBackButton()
		{
			if (_backButton == null)
				return;

			if (ShowBackButton)
				_backButton.Visibility = Visibility.Visible;
			else
				_backButton.Visibility = InvisibleBackButtonCollapsed ? Visibility.Collapsed : Visibility.Visible;

			_backButton.Opacity = ShowBackButton ? 1 : 0;
		}

		void UpdateTitleViewPresenter()
		{
			if (TitleView == null)
			{
				TitleViewVisibility = Visibility.Collapsed;

				if (_titleViewPresenter != null)
					_titleViewPresenter.Loaded -= OnTitleViewPresenterLoaded;
			}
			else
			{
				TitleViewVisibility = Visibility.Visible;

				if (_titleViewPresenter != null)
					_titleViewPresenter.Loaded += OnTitleViewPresenterLoaded;
			}
		}
	}
}
