using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using flare_app.ViewModels;
using flare_app.Views;
using flare_app.Services;
using Grpc.Net.Client;
using flare_csharp;

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
                    fonts.AddFont("IBMPlexMono-Regular.ttf", "IBMPlexMono");
                    fonts.AddFont("IBMPlexMono-Bold.ttf", "IBMPlexMonoBold");
                    fonts.AddFont("IBMPlexMono-Italic.ttf", "IBMPlexMonoItalic");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
			builder.Services.AddScoped<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();
            //builder.Services.AddSingleton<ChatPage>();
            builder.Services.AddSingleton<MessagingService>();
            return builder.Build();

        }

        public static async void ErrorToast(string message)
        {
            // [DEV_NOTE]: should not make default cancellation token source, add timespan to the constructor
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;

            var toast = Toast.Make(message, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }

    }
}
