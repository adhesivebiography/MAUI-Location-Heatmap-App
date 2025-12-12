using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Storage;
using MauiApp1.Data;

namespace MauiApp1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register SQLite database (Singleton)
        builder.Services.AddSingleton<LocationDatabase>(_ =>
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "locations.db3");
            return new LocationDatabase(dbPath);
        });

        // Register MainPage so DI can construct it (because it needs LocationDatabase)
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
