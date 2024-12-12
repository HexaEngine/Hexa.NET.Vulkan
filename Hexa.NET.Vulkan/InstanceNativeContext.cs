namespace Hexa.NET.Vulkan
{
    using HexaGen.Runtime;
    using System;

    public unsafe class InstanceNativeContext : INativeContext
    {
        private readonly VkInstance instance;

        public InstanceNativeContext(VkInstance instance)
        {
            this.instance = instance;
        }

        public nint GetProcAddress(string procName)
        {
            return (nint)Vulkan.GetInstanceProcAddr(instance, procName);
        }

        public bool IsExtensionSupported(string extensionName)
        {
            throw new NotImplementedException();
        }

        public bool TryGetProcAddress(string procName, out nint procAddress)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }

    public unsafe class DeviceNativeContext : INativeContext
    {
        private readonly VkDevice device;

        public DeviceNativeContext(VkDevice device)
        {
            this.device = device;
        }

        public nint GetProcAddress(string procName)
        {
            return (nint)Vulkan.GetDeviceProcAddr(device, procName);
        }

        public bool IsExtensionSupported(string extensionName)
        {
            throw new NotImplementedException();
        }

        public bool TryGetProcAddress(string procName, out nint procAddress)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}