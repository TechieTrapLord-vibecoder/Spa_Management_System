namespace Spa_Management_System
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage()) 
            { 
                Title = "Kaye Spa Management System" 
            };

            // Maximize window on startup
            window.Created += (s, e) =>
            {
#if WINDOWS
                var nativeWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                if (nativeWindow != null)
                {
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                    var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                    
                    if (appWindow != null)
                    {
                        // Get the display area where the window is located
                        var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
                        
                        if (displayArea != null)
                        {
                            // Set window to fill the entire work area (maximized)
                            var workArea = displayArea.WorkArea;
                            appWindow.MoveAndResize(new Windows.Graphics.RectInt32(
                                workArea.X,
                                workArea.Y,
                                workArea.Width,
                                workArea.Height
                            ));
                        }
                    }
                }
#elif MACCATALYST || MACOS
                // For macOS/Mac Catalyst, maximize the window
                var disp = DeviceDisplay.Current.MainDisplayInfo;
                window.Width = disp.Width / disp.Density;
                window.Height = disp.Height / disp.Density;
                window.X = 0;
                window.Y = 0;
#endif
            };

            return window;
        }
    }
}
