namespace Hexa.NET.Vulkan
{
    public unsafe partial class Device
    {
        public Device(Instance instance, VkPhysicalDevice physicalDevice, VkDeviceCreateInfo createInfo, VkAllocationCallbacks* allocationCallbacks)
        {
            ownsDevice = true;
            VkDevice device;
            instance.CreateDevice(physicalDevice, &createInfo, allocationCallbacks, &device);
            this.device = device;
            this.allocationCallbacks = allocationCallbacks;
            context = new(device);
            InitTable();
        }

        public Device(VkDevice device, VkAllocationCallbacks* allocationCallbacks, bool transferOwnership)
        {
            ownsDevice = transferOwnership;
            this.device = device;
            this.allocationCallbacks = allocationCallbacks;
            context = new(device);
            InitTable();
        }
    }
}