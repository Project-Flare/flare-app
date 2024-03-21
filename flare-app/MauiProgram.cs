using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Flare;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using flare_app.ViewModels;

namespace flare_app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<Client>();

            builder.Services.AddSingleton<Views.DiscoveryPage>();
            builder.Services.AddSingleton<MainViewModel>();

            builder.Services.AddSingleton<Views.MainPage>();

            return builder.Build();
        }
        public static async void ErrorToast(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;

            var toast = Toast.Make(message, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}
