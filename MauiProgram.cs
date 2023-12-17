using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace CsdbMigration
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
                    fonts.AddFont("AvenirNextLTPro-Bold.otf", "AvenirNextLTPro-Bold");
                    fonts.AddFont("AvenirNextLTPro-BoldCn.otf", "AvenirNextLTPro-BoldCn");
                    fonts.AddFont("AvenirNextLTPro-BoldCnIt.otf", "AvenirNextLTPro-BoldCnIt");
                    fonts.AddFont("AvenirNextLTPro-BoldIt.otf", "AvenirNextLTPro-BoldIt");
                    fonts.AddFont("AvenirNextLTPro-Cn.otf", "AvenirNextLTPro-Cn");
                    fonts.AddFont("AvenirNextLTPro-CnIt.otf", "AvenirNextLTPro-CnIt");
                    fonts.AddFont("AvenirNextLTPro-Demi.otf", "AvenirNextLTPro-Demi");
                    fonts.AddFont("AvenirNextLTPro-DemiCn.otf", "AvenirNextLTPro-DemiCn");
                    fonts.AddFont("AvenirNextLTPro-DemiCnIt.otf", "AvenirNextLTPro-DemiCnIt");
                    fonts.AddFont("AvenirNextLTPro-DemiIt.otf", "AvenirNextLTPro-DemiIt");
                    fonts.AddFont("AvenirNextLTPro-Heavy.otf", "AvenirNextLTPro-Heavy");
                    fonts.AddFont("AvenirNextLTPro-HeavyCn.otf", "AvenirNextLTPro-HeavyCn");
                    fonts.AddFont("AvenirNextLTPro-HeavyCnIt.otf", "AvenirNextLTPro-HeavyCnIt");
                    fonts.AddFont("AvenirNextLTPro-HeavyIt.otf", "AvenirNextLTPro-HeavyIt");
                    fonts.AddFont("AvenirNextLTPro-It.otf", "AvenirNextLTPro-It");
                    fonts.AddFont("AvenirNextLTPro-Medium.otf", "AvenirNextLTPro-Medium");
                    fonts.AddFont("AvenirNextLTPro-MediumCn.otf", "AvenirNextLTPro-MediumCn");
                    fonts.AddFont("AvenirNextLTPro-MediumCnIt.otf", "AvenirNextLTPro-MediumCnIt");
                    fonts.AddFont("AvenirNextLTPro-MediumIt.otf", "AvenirNextLTPro-MediumIt");
                    fonts.AddFont("AvenirNextLTPro-Regular.otf", "AvenirNextLTPro-Regular");
                    fonts.AddFont("AvenirNextLTPro-UltLt.otf", "AvenirNextLTPro-UltLt");
                    fonts.AddFont("AvenirNextLTPro-UltLtCn.otf", "AvenirNextLTPro-UltLtCn");
                    fonts.AddFont("AvenirNextLTPro-UltLtCnIt.otf", "AvenirNextLTPro-UltLtCnIt");
                    fonts.AddFont("AvenirNextLTPro-UltLtIt.otf", "AvenirNextLTPro-UltLtIt");
                }).UseMauiCommunityToolkit();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
