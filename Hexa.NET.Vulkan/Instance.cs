namespace Hexa.NET.Vulkan
{
    public unsafe partial class Instance
    {
        public Instance(VkInstanceCreateInfo createInfo, VkAllocationCallbacks* allocationCallbacks)
        {
            ownsInstance = true;
            VkInstance instance;
            Vulkan.CreateInstance(&createInfo, allocationCallbacks, &instance);
            this.instance = instance;
            this.allocationCallbacks = allocationCallbacks;
            context = new(instance);
            InitTable();
        }

        public Instance(VkInstance instance, VkAllocationCallbacks* allocationCallbacks, bool transferOwnership)
        {
            ownsInstance = transferOwnership;
            this.instance = instance;
            this.allocationCallbacks = allocationCallbacks;
            context = new(instance);
            InitTable();
        }

        public Device CreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo createInfo)
        {
            return new(this, physicalDevice, createInfo, allocationCallbacks);
        }
    }
}