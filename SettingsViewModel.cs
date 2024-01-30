using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using BlazorBootstrap;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CsdbMigration.Components.Pages;
using CsdbMigration.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Platform;
namespace CsdbMigration;

public partial class SettingsViewModel(
    ConcurrentDictionary<DateTime, LogRecord> logs,
    ILogger<ICsdbMigrationViewModel> logger,
    ModalService modalService,
    IConfiguration configuration
)
    : ObservableObject, ICsdbMigrationViewModel
{
    public string SelectedLanguage { get; set; } = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    public static string HashiCorpClientSecret
    {
        get => string.IsNullOrEmpty(Preferences.Default.Get("HashiCorpClientSecret", string.Empty)) ? string.Empty : string.Concat(Enumerable.Repeat('*', 64));
        set => Preferences.Default.Set("HashiCorpClientSecret", value);
    }
    public void OnSubmitLanguage(EventArgs args)
    {
        Debug.Print($"OnSubmitLanguage: {SelectedLanguage}");
        Debug.Print($"Current Culture: {CultureInfo.CurrentCulture}");
        Debug.Print($"Current UI Culture: {CultureInfo.CurrentUICulture}");
        Debug.Print($"Thread Current Culture: {Thread.CurrentThread.CurrentCulture}");
        Debug.Print($"Thread Current UI Culture: {Thread.CurrentThread.CurrentUICulture}");
        var newCulture = CultureInfo.CreateSpecificCulture(SelectedLanguage);
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = Thread.CurrentThread.CurrentUICulture = newCulture;
    }

}