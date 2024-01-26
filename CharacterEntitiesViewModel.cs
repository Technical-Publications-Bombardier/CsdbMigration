using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BlazorBootstrap;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CsdbMigration.Shared;
using Microsoft.Extensions.Logging;
namespace CsdbMigration;

public partial class CharacterEntitiesViewModel(
    ConcurrentDictionary<DateTime, LogRecord> logs,
    ILogger<ICsdbMigrationViewModel> logger,
    ModalService modalService
    )
    : ObservableObject, ICsdbMigrationViewModel
{

}