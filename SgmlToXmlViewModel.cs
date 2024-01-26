using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using BlazorBootstrap;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CsdbMigration.Components.Utilities;
using CsdbMigration.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Common;
namespace CsdbMigration;

public class SgmlToXmlViewModel(
    ConcurrentDictionary<DateTime, LogRecord> logs,
    ILogger<ICsdbMigrationViewModel> logger,
    ModalService modalService,
    IConfiguration configuration
)
    : ObservableObject, ICsdbMigrationViewModel
{
    public bool ConvertIsEnabled => SelectedSgmlFiles.Count > 0;

    public string ConvertTooltip { get; set; } = "Convert the selected SGML files into well-formed XML";

    private readonly FilePickerFileType _sgmlFileType = new(
        new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "text/sgml" } }, // UTType values
            { DevicePlatform.Android, new[] { "text/sgml" } }, // MIME type
            { DevicePlatform.WinUI, new[] { ".sgm", ".sgml" } }, // file extension
            { DevicePlatform.Tizen, new[] { "*/*" } },
            { DevicePlatform.macOS, new[] { "sgm", "sgml" } } // UTType values
        });
    private HashSet<FileInfo> SelectedSgmlFiles { get; set; } = [];
    public string EmptyText { get; set; } = "No SGML file(s) chosen";
    public Grid<FileInfo>? SgmlFilesGrid { get; set; }
    public bool ByFolder { get; set; }
    public IEnumerable<FileInfo> SgmlFiles { get; set; } = [];
    public void OnSelectedSgmlFilesChanged(HashSet<FileInfo> sgmlFiles)
    {
        SelectedSgmlFiles = sgmlFiles;
        if (Debugger.IsAttached)
            Debug.WriteLine($"OnSelectedSgmlFilesChanged: {sgmlFiles.Count} files");
        if (sgmlFiles.Count > 1)
            ConvertTooltip = "Convert the selected SGML files into well-formed XML";
        else
            ConvertTooltip = "Convert the selected SGML file into well-formed XML";
    }
    public async Task SelectSgmlFiles(EventArgs eventArgs)
    {
        // show the Maui Blazor file picker
        PickOptions options = new()
        {
            PickerTitle = "Please select SGML files",
            FileTypes = _sgmlFileType
        };
        var results = await FilePicker.Default.PickMultipleAsync(options);
        SgmlFiles = results.Select(x => new FileInfo(x.FullPath));
        OnPropertyChanged(nameof(SgmlFiles));
        if (SgmlFilesGrid is not null)
            await SgmlFilesGrid.RefreshDataAsync();
    }

    public async Task SelectFolder(EventArgs eventArgs)
    {
        if (await FolderPicker.Default.PickAsync() is not { Folder: not null } result)
            return;
        // recursively search for SGML files starting in result
        SgmlFiles = Directory.EnumerateFiles(result.Folder.Path, "*.sgm", SearchOption.AllDirectories).Select(x => new FileInfo(x));
    }

    public async Task<bool> Convert(EventArgs eventArgs)
    {
        if (Debugger.IsAttached)
            Debug.WriteLine($"Convert: {SelectedSgmlFiles.Count} files");
        using var client = new SshClient(configuration["Ssh:Hostname"], configuration["Ssh:Username"], configuration["Ssh:Password"]);
        try
        {
            await client.ConnectAsync(default);
        }
        catch (InvalidOperationException exception) when (!Debugger.IsAttached)
        {
            var option = new ModalOption
            {
                Title = "SSH Connection Error",
                IsVerticallyCentered = true,
                Message = $"{exception.GetType()}: {exception.Message}",
                Type = ModalType.Danger
            };
            option.Message += exception.InnerException is null ? string.Empty : $"{Environment.NewLine}{exception.InnerException.GetType()}: {exception.InnerException.Message}";
            await modalService.ShowAsync(option);
        }
        catch (ObjectDisposedException exception) when (!Debugger.IsAttached)
        {
            var option = new ModalOption
            {
                Title = "SSH Connection Error",
                IsVerticallyCentered = true,
                Message = $"{exception.GetType()}: {exception.Message}",
                Type = ModalType.Danger
            };
            option.Message += exception.InnerException is null ? string.Empty : $"{Environment.NewLine}{exception.InnerException.GetType()}: {exception.InnerException.Message}";
            await modalService.ShowAsync(option);
        }
        catch (SocketException exception) when (!Debugger.IsAttached)
        {
            var option = new ModalOption
            {
                Title = "SSH Connection Error",
                IsVerticallyCentered = true,
                Message = $"{exception.GetType()}: {exception.Message}",
                Type = ModalType.Danger
            };
            option.Message += exception.InnerException is null ? string.Empty : $"{Environment.NewLine}{exception.InnerException.GetType()}: {exception.InnerException.Message}";
            await modalService.ShowAsync(option);
        }
        catch (SshConnectionException exception) when (!Debugger.IsAttached)
        {
            var option = new ModalOption
            {
                Title = "SSH Connection Error",
                IsVerticallyCentered = true,
                Message = $"{exception.GetType()}: {exception.Message}",
                Type = ModalType.Danger
            };
            option.Message += exception.InnerException is null ? string.Empty : $"{Environment.NewLine}{exception.InnerException.GetType()}: {exception.InnerException.Message}";
            await modalService.ShowAsync(option);
        }
        catch (SshAuthenticationException exception) when (!Debugger.IsAttached)
        {
            var option = new ModalOption
            {
                Title = "SSH Connection Error",
                IsVerticallyCentered = true,
                Message = $"{exception.GetType()}: {exception.Message}",
                Type = ModalType.Danger
            };
            option.Message += exception.InnerException is null ? string.Empty : $"{Environment.NewLine}{exception.InnerException.GetType()}: {exception.InnerException.Message}";
            await modalService.ShowAsync(option);
        }
        catch (ProxyException exception) when (!Debugger.IsAttached)
        {
            var option = new ModalOption
            {
                Title = "SSH Connection Error",
                IsVerticallyCentered = true,
                Message = $"{exception.GetType()}: {exception.Message}",
                Type = ModalType.Danger
            };
            option.Message += exception.InnerException is null ? string.Empty : $"{Environment.NewLine}{exception.InnerException.GetType()}: {exception.InnerException.Message}";
            await modalService.ShowAsync(option);
        }
        var output = client.CreateCommand("bash -c 'echo hello'").Execute();
        Debug.Assert(output.Equals("hello", StringComparison.Ordinal));
        client.Disconnect();
        client.Dispose();
        var sb = new StringBuilder(SelectedSgmlFiles.Count);
        foreach (var sgmlFile in SelectedSgmlFiles)
        {
            string uncPath;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (sgmlFile.Directory is not { } dir)
                    continue;
                uncPath = NetworkDriveResolver.ResolveWindowsDriveToUNCPath(dir.Root.Name);
            }
        }
        // TODO: execute bash script
        return true;
    }
}