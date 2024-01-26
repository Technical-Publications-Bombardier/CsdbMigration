using System.Collections.Concurrent;
using BlazorBootstrap;
using CommunityToolkit.Mvvm.ComponentModel;
using CsdbMigration.Shared;
using Microsoft.Extensions.Logging;

namespace CsdbMigration;

public class GraphicsFileExtensionsViewModel(
    ConcurrentDictionary<DateTime, LogRecord> logs,
    ILogger<ICsdbMigrationViewModel> logger,
    ModalService modalService
)
    : ObservableObject, ICsdbMigrationViewModel
{
}