using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace maui_app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register navigation routes
            Routing.RegisterRoute("TechnicianJobDetailPage", typeof(Pages.TechnicianJobDetailPage));
            Routing.RegisterRoute("TechnicianJobPartsPage", typeof(Pages.TechnicianJobPartsPage));

            return builder.Build();
        }
    }
}
