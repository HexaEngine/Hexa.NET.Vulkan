namespace Hexa.NET.Vulkan
{
    using HexaGen.Runtime;
    using System.Runtime.InteropServices;

    public static unsafe partial class Vulkan
    {
        static Vulkan()
        {
            //InitApi();
        }

        public static uint MakeApiVersion(byte variant, byte major, byte minor, byte patch)
        {
            return (uint)(((variant) << 29) | ((major) << 22) | ((minor) << 12) | (patch));
        }

        public static readonly uint VkApiVersion10 = MakeApiVersion(0, 1, 0, 0);

        public static readonly uint VkApiVersion11 = MakeApiVersion(0, 1, 1, 0);

        public static readonly uint VkApiVersion12 = MakeApiVersion(0, 1, 2, 0);

        public static readonly uint VkApiVersion13 = MakeApiVersion(0, 1, 3, 0);

        public static readonly uint VkApiVersion14 = MakeApiVersion(0, 1, 4, 0);

        public static string GetLibraryName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "vulkan-1";
            }

            return "libvulkan";
        }
    }
}