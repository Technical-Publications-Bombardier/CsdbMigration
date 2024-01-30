using System.Collections.Concurrent;
using System.Reflection;
using BlazorBootstrap;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using CsdbMigration.Resources;
using CsdbMigration.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
// ReSharper disable once RedundantUsingDirective
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace CsdbMigration;

public static class MauiProgram
{
    private static readonly string[] SupportedLocales = ["en", "fr"];
#if WINDOWS
        private static Exception _lastFirstChanceException;
#endif
#if !DEBUG
    public static event UnhandledExceptionEventHandler? UnhandledException;
#endif

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        var config = new ConfigurationBuilder()
            .AddJsonStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("CsdbMigration.appsettings.json")!)
            .Build();
        builder.Configuration.AddConfiguration(config);
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddBlazorBootstrap();
        // Add localization services
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        builder.Services.AddSingleton<ConcurrentDictionary<DateTime, LogRecord>>();
        builder.Services.AddSingleton<ModalService>();
        // Register the logger as a singleton
        builder.Services.AddSingleton<ILogger>(services =>
        {
            var logs = services.GetRequiredService<ConcurrentDictionary<DateTime, LogRecord>>();
            return new BaLogger(logs, LogLevel.Trace);
        });
        builder.Services.AddSingleton<ILogger<ICsdbMigrationViewModel>>(services =>
        {
            var logs = services.GetRequiredService<ConcurrentDictionary<DateTime, LogRecord>>();
            return new BaLogger(logs, LogLevel.Trace);
        });
        // Register CharacterEntitiesViewModel
        builder.Services.AddSingleton<ICsdbMigrationViewModel, CharacterEntitiesViewModel>(services =>
        {
            var logs = services.GetRequiredService<ConcurrentDictionary<DateTime, LogRecord>>();
            var logger = services.GetRequiredService<ILogger<ICsdbMigrationViewModel>>();
            var modalService = services.GetRequiredService<ModalService>();
            return new CharacterEntitiesViewModel(logs, logger, modalService);
        });
        // Register GraphicsFileExtensionsViewModel
        builder.Services.AddSingleton<ICsdbMigrationViewModel, GraphicsFileExtensionsViewModel>(services =>
        {
            var logs = services.GetRequiredService<ConcurrentDictionary<DateTime, LogRecord>>();
            var logger = services.GetRequiredService<ILogger<ICsdbMigrationViewModel>>();
            var modalService = services.GetRequiredService<ModalService>();
            return new GraphicsFileExtensionsViewModel(logs, logger, modalService);
        });
        // Register SettingsViewModel
        builder.Services.AddSingleton<ICsdbMigrationViewModel, SettingsViewModel>(services =>
        {
            var logs = services.GetRequiredService<ConcurrentDictionary<DateTime, LogRecord>>();
            var logger = services.GetRequiredService<ILogger<ICsdbMigrationViewModel>>();
            var modalService = services.GetRequiredService<ModalService>();
            return new SettingsViewModel(logs, logger, modalService, config);
        });
        // Register SgmlToXmlViewModel
        builder.Services.AddSingleton<ICsdbMigrationViewModel, SgmlToXmlViewModel>(services =>
        {
            var logs = services.GetRequiredService<ConcurrentDictionary<DateTime, LogRecord>>();
            var logger = services.GetRequiredService<ILogger<ICsdbMigrationViewModel>>();
            var modalService = services.GetRequiredService<ModalService>();
            return new SgmlToXmlViewModel(logs, logger, modalService, config);
        });
        // Register the individual view models as singletons
        builder.Services.AddSingleton<CharacterEntitiesViewModel>();
        builder.Services.AddSingleton<GraphicsFileExtensionsViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SgmlToXmlViewModel>();
        builder.Services.AddSingleton(FolderPicker.Default);

        // Register the MainPage as transient to make sure it can resolve the IFolderPicker dependency.
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddBlazorBootstrap();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
        builder.Configuration.AddUserSecrets(typeof(App).Assembly);
#else
        builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
        builder.Services.AddLogging(logging => logging.AddApplicationInsights(
            config => config.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"],
            options => { }));
        builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("CsdbMigration", LogLevel.Trace);
        builder.Services.AddApplicationInsightsTelemetryWorkerService();
        AppDomain.CurrentDomain.UnhandledException += (sender, args) => UnhandledException?.Invoke(sender, args);
        TaskScheduler.UnobservedTaskException += (sender, args) =>
            UnhandledException?.Invoke(sender!, new UnhandledExceptionEventArgs(args.Exception, false));
#if IOS || MACCATALYST
            ObjCRuntime.Runtime.MarshalManagedException += (_, args) => args.ExceptionMode =
 ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
#elif ANDROID
        AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
        {
            args.Handled = true; // Suppress the exception from being propagated to the managed thread
            UnhandledException?.Invoke(sender!, new UnhandledExceptionEventArgs(args.Exception, true));
        };
#elif WINDOWS
            AppDomain.CurrentDomain.FirstChanceException += (_, args) => _lastFirstChanceException = args.Exception;
            Microsoft.UI.Xaml.Application.Current.UnhandledException += (sender, args) => {
                var exception = args.Exception;
                if (exception.StackTrace is null)
                {
                    exception = _lastFirstChanceException;
                }
                UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(exception, true));
            };
#endif
#endif
        var host = builder.Build();
        var logger = host.Services.GetRequiredService<ILogger<ICsdbMigrationViewModel>>();
        logger.LogDebug("MauiProgram.CreateMauiApp()");
        return host;
    }
}