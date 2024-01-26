using System.Runtime.InteropServices;
namespace CsdbMigration.Components.Utilities;

public static partial class NetworkDriveResolver
{
    [LibraryImport("mpr.dll", EntryPoint = "WNetGetUniversalNameW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int WNetGetUniversalName(
    string lpLocalPath,
    int dwInfoLevel,
    IntPtr lpBuffer,
    ref int lpBufferSize);

    // Define the info level
    const int UNIVERSAL_NAME_INFO_LEVEL = 0x00000001;
    const int REMOTE_NAME_INFO_LEVEL = 0x00000002;

    // Define the error codes
    const int NO_ERROR = 0;
    const int ERROR_MORE_DATA = 234;

    // Define the structure to receive the data
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct UNIVERSAL_NAME_INFO
    {
        public string lpUniversalName;
    }

    public static string ResolveWindowsDriveToUNCPath(string driveLetter)
    {
        int bufferSize = 512;
        IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
        try
        {
            int result = WNetGetUniversalName(driveLetter, UNIVERSAL_NAME_INFO_LEVEL, buffer, ref bufferSize);

            if (result == NO_ERROR)
            {
                UNIVERSAL_NAME_INFO uni = Marshal.PtrToStructure<UNIVERSAL_NAME_INFO>(buffer);
                return uni.lpUniversalName;
            }
            else if (result == ERROR_MORE_DATA)
            {
                // Allocate more memory
                buffer = Marshal.ReAllocHGlobal(buffer, (IntPtr)bufferSize);
                result = WNetGetUniversalName(driveLetter, UNIVERSAL_NAME_INFO_LEVEL, buffer, ref bufferSize);

                if (result == NO_ERROR)
                {
                    UNIVERSAL_NAME_INFO uni = Marshal.PtrToStructure<UNIVERSAL_NAME_INFO>(buffer);
                    return uni.lpUniversalName;
                }
                else
                {
                    throw new InvalidOperationException("Failed to get the UNC path for the drive letter.");
                }
            }
            else
            {
                throw new InvalidOperationException("Failed to get the UNC path for the drive letter.");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}