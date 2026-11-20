using Hexa.NET.Vulkan;
using Hexa.NET.Vulkan.KHR;
using Hexa.NET.SDL3;
using System.Runtime.InteropServices;

using SDLVkInstance = Hexa.NET.SDL3.VkInstance;
using SDLVkSurfaceKHR = Hexa.NET.SDL3.VkSurfaceKHR;
using VkInstance = Hexa.NET.Vulkan.VkInstance;
using VkSurfaceKHR = Hexa.NET.Vulkan.KHR.VkSurfaceKHR;
using VkPhysicalDevice = Hexa.NET.Vulkan.VkPhysicalDevice;
using VkAllocationCallbacks = Hexa.NET.Vulkan.VkAllocationCallbacks;

Vulkan.InitApi();

if (!SDL.Init((uint)SDLInitFlags.Video))
{
    Console.WriteLine($"SDL_Init failed: {SDL.GetErrorS()}");
    return;
}

SDLWindowPtr window = SDL.CreateWindow("Hexa.NET.Vulkan validation"u8, 640, 480, (ulong)SDLWindowFlags.Vulkan);
if (window.IsNull)
{
    Console.WriteLine($"SDL_CreateWindow failed: {SDL.GetErrorS()}");
    return;
}

unsafe
{
    uint extCount = 0;
    byte** sdlExtensions = SDL.VulkanGetInstanceExtensions(&extCount);
    if (sdlExtensions == null)
    {
        Console.WriteLine($"SDL_Vulkan_GetInstanceExtensions failed: {SDL.GetErrorS()}");
        return;
    }

    byte* appName = (byte*)Marshal.StringToHGlobalAnsi("Hexa.NET.Vulkan.Example");

    VkApplicationInfo appInfo = new()
    {
        SType = VkStructureType.ApplicationInfo,
        PApplicationName = appName,
        ApplicationVersion = Vulkan.MakeApiVersion(0, 1, 0, 0),
        PEngineName = appName,
        EngineVersion = Vulkan.MakeApiVersion(0, 1, 0, 0),
        ApiVersion = Vulkan.VkApiVersion10,
    };

    VkInstanceCreateInfo instanceCreateInfo = new()
    {
        SType = VkStructureType.InstanceCreateInfo,
        PApplicationInfo = &appInfo,
        EnabledExtensionCount = extCount,
        PpEnabledExtensionNames = sdlExtensions,
    };

    VkInstance rawInstance;
    VkResult result = Vulkan.CreateInstance(ref instanceCreateInfo, null, &rawInstance);
    Marshal.FreeHGlobal((nint)appName);
    if (result != VkResult.Success)
    {
        Console.WriteLine($"vkCreateInstance failed: {result}");
        return;
    }

    Instance instance = new(rawInstance, null, true);
    Console.WriteLine("Instance created.");

    // SDL3 generates its own structurally identical Vulkan handle types; bridge via the raw nint.
    SDLVkInstance sdlInstanceHandle = new(rawInstance.Handle);
    SDLVkSurfaceKHR sdlSurface;
    if (!SDL.VulkanCreateSurface(window, sdlInstanceHandle, null, &sdlSurface))
    {
        Console.WriteLine($"SDL_Vulkan_CreateSurface failed: {SDL.GetErrorS()}");
        return;
    }

    VkSurfaceKHR surface = new(sdlSurface.Handle);
    Console.WriteLine("Surface created.");

    VKKHRSurface surfaceExt = new();
    surfaceExt.InitApi();

    uint deviceCount = 0;
    instance.EnumeratePhysicalDevices(rawInstance, ref deviceCount, (VkPhysicalDevice*)null);
    if (deviceCount == 0)
    {
        Console.WriteLine("No Vulkan-capable physical devices found.");
        return;
    }

    VkPhysicalDevice[] physicalDevices = new VkPhysicalDevice[deviceCount];
    fixed (VkPhysicalDevice* pDevices = physicalDevices)
    {
        instance.EnumeratePhysicalDevices(rawInstance, ref deviceCount, pDevices);
    }

    VkPhysicalDevice physicalDevice = default;
    uint queueFamilyIndex = uint.MaxValue;

    for (int d = 0; d < physicalDevices.Length && queueFamilyIndex == uint.MaxValue; d++)
    {
        VkPhysicalDevice pd = physicalDevices[d];

        uint qfCount = 0;
        instance.GetPhysicalDeviceQueueFamilyProperties(pd, ref qfCount, (VkQueueFamilyProperties*)null);
        VkQueueFamilyProperties[] qfProps = new VkQueueFamilyProperties[qfCount];
        fixed (VkQueueFamilyProperties* pQf = qfProps)
        {
            instance.GetPhysicalDeviceQueueFamilyProperties(pd, ref qfCount, pQf);
        }

        for (uint i = 0; i < qfCount; i++)
        {
            bool graphics = (qfProps[i].QueueFlags & VkQueueFlags.GraphicsBit) != 0;
            VkBool32 presentSupported = false;
            surfaceExt.GetPhysicalDeviceSurfaceSupportKHR(pd, i, surface, ref presentSupported);

            if (graphics && presentSupported)
            {
                physicalDevice = pd;
                queueFamilyIndex = i;
                break;
            }
        }
    }

    if (queueFamilyIndex == uint.MaxValue)
    {
        Console.WriteLine("No queue family supporting graphics + present was found.");
        return;
    }

    Console.WriteLine($"Using physical device with queue family {queueFamilyIndex}.");

    float priority = 1.0f;
    VkDeviceQueueCreateInfo queueCreateInfo = new()
    {
        SType = VkStructureType.DeviceQueueCreateInfo,
        QueueFamilyIndex = queueFamilyIndex,
        QueueCount = 1,
        PQueuePriorities = &priority,
    };

    byte* swapchainExtensionName = (byte*)Marshal.StringToHGlobalAnsi("VK_KHR_swapchain");

    VkDeviceCreateInfo deviceCreateInfo = new()
    {
        SType = VkStructureType.DeviceCreateInfo,
        QueueCreateInfoCount = 1,
        PQueueCreateInfos = &queueCreateInfo,
        EnabledExtensionCount = 1,
        PpEnabledExtensionNames = &swapchainExtensionName,
    };

    VkDevice rawDevice;
    result = instance.CreateDevice(physicalDevice, ref deviceCreateInfo, null, &rawDevice);
    Marshal.FreeHGlobal((nint)swapchainExtensionName);
    if (result != VkResult.Success)
    {
        Console.WriteLine($"vkCreateDevice failed: {result}");
        return;
    }

    Device device = new(rawDevice, null, true);
    Console.WriteLine("Device created.");

    VKKHRSwapchain swapchainExt = new();
    swapchainExt.InitApi();

    VkSurfaceCapabilitiesKHR caps = default;
    surfaceExt.GetPhysicalDeviceSurfaceCapabilitiesKHR(physicalDevice, surface, ref caps);

    uint formatCount = 0;
    surfaceExt.GetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, surface, ref formatCount, (VkSurfaceFormatKHR*)null);
    VkSurfaceFormatKHR[] formats = new VkSurfaceFormatKHR[formatCount];
    fixed (VkSurfaceFormatKHR* pFormats = formats)
    {
        surfaceExt.GetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, surface, ref formatCount, pFormats);
    }

    VkSurfaceFormatKHR chosenFormat = formats[0];
    uint minImageCount = caps.MaxImageCount > 0 && caps.MinImageCount + 1 > caps.MaxImageCount
        ? caps.MaxImageCount
        : caps.MinImageCount + 1;

    // The call below used to fail to compile (duplicate "pCreateInfo" from vk.xml's api="vulkansc" variant)
    // and then crash at runtime (every KHR extension's function table collided into one shared class).
    VkSwapchainCreateInfoKHR swapchainCreateInfo = new()
    {
        SType = VkStructureType.SwapchainCreateInfoKhr,
        Surface = surface,
        MinImageCount = minImageCount,
        ImageFormat = chosenFormat.Format,
        ImageColorSpace = chosenFormat.ColorSpace,
        ImageExtent = caps.CurrentExtent,
        ImageArrayLayers = 1,
        ImageUsage = VkImageUsageFlags.ColorAttachmentBit,
        ImageSharingMode = VkSharingMode.Exclusive,
        PreTransform = caps.CurrentTransform,
        CompositeAlpha = VkCompositeAlphaFlagsKHR.OpaqueBitKhr,
        PresentMode = VkPresentModeKHR.FifoKhr,
        Clipped = true,
    };

    VkSwapchainKHR swapchain;
    result = swapchainExt.CreateSwapchainKHR(rawDevice, ref swapchainCreateInfo, null, &swapchain);
    if (result != VkResult.Success)
    {
        Console.WriteLine($"vkCreateSwapchainKHR failed: {result}");
        return;
    }

    Console.WriteLine($"Swapchain created with {minImageCount} images at {caps.CurrentExtent.Width}x{caps.CurrentExtent.Height}.");

    uint imageCount = 0;
    swapchainExt.GetSwapchainImagesKHR(rawDevice, swapchain, ref imageCount, (VkImage*)null);
    VkImage[] swapchainImages = new VkImage[imageCount];
    fixed (VkImage* pImages = swapchainImages)
    {
        swapchainExt.GetSwapchainImagesKHR(rawDevice, swapchain, ref imageCount, pImages);
    }

    VkQueue queue;
    device.GetDeviceQueue(rawDevice, queueFamilyIndex, 0, &queue);

    VkCommandPoolCreateInfo poolCreateInfo = new()
    {
        SType = VkStructureType.CommandPoolCreateInfo,
        Flags = VkCommandPoolCreateFlags.ResetCommandBufferBit,
        QueueFamilyIndex = queueFamilyIndex,
    };

    VkCommandPool commandPool;
    device.CreateCommandPool(rawDevice, ref poolCreateInfo, null, &commandPool);

    VkCommandBufferAllocateInfo cmdAllocInfo = new()
    {
        SType = VkStructureType.CommandBufferAllocateInfo,
        CommandPool = commandPool,
        Level = VkCommandBufferLevel.Primary,
        CommandBufferCount = 1,
    };

    VkCommandBuffer commandBuffer;
    device.AllocateCommandBuffers(rawDevice, ref cmdAllocInfo, &commandBuffer);

    VkSemaphoreCreateInfo semaphoreCreateInfo = new() { SType = VkStructureType.SemaphoreCreateInfo };
    VkSemaphore imageAvailable;
    VkSemaphore renderFinished;
    device.CreateSemaphore(rawDevice, ref semaphoreCreateInfo, null, &imageAvailable);
    device.CreateSemaphore(rawDevice, ref semaphoreCreateInfo, null, &renderFinished);

    VkFenceCreateInfo fenceCreateInfo = new() { SType = VkStructureType.FenceCreateInfo, Flags = VkFenceCreateFlags.SignaledBit };
    VkFence frameFence;
    device.CreateFence(rawDevice, ref fenceCreateInfo, null, &frameFence);

    VkImageSubresourceRange subresourceRange = new()
    {
        AspectMask = VkImageAspectFlags.ColorBit,
        BaseMipLevel = 0,
        LevelCount = 1,
        BaseArrayLayer = 0,
        LayerCount = 1,
    };

    uint windowId = SDL.GetWindowID(window);
    SDLEvent sdlEvent = default;
    bool exiting = false;
    float hue = 0.0f;

    Console.WriteLine("Clearing the swapchain image each frame. Close the window to exit.");

    while (!exiting)
    {
        SDL.PumpEvents();

        while (SDL.PollEvent(ref sdlEvent))
        {
            switch ((SDLEventType)sdlEvent.Type)
            {
                case SDLEventType.Quit:
                case SDLEventType.Terminating:
                    exiting = true;
                    break;

                case SDLEventType.WindowCloseRequested:
                    if (sdlEvent.Window.WindowID == windowId)
                    {
                        exiting = true;
                    }
                    break;
            }
        }

        if (exiting)
        {
            break;
        }

        device.WaitForFences(rawDevice, 1, &frameFence, true, ulong.MaxValue);
        device.ResetFences(rawDevice, 1, &frameFence);

        uint imageIndex = 0;
        result = swapchainExt.AcquireNextImageKHR(rawDevice, swapchain, ulong.MaxValue, imageAvailable, default, ref imageIndex);
        if (result != VkResult.Success && result != VkResult.SuboptimalKhr)
        {
            Console.WriteLine($"vkAcquireNextImageKHR failed: {result}");
            break;
        }

        hue += 0.01f;
        VkClearColorValue clearColor = default;
        clearColor.Float32[0] = (MathF.Sin(hue) + 1.0f) * 0.5f;
        clearColor.Float32[1] = (MathF.Sin(hue + 2.0f) + 1.0f) * 0.5f;
        clearColor.Float32[2] = (MathF.Sin(hue + 4.0f) + 1.0f) * 0.5f;
        clearColor.Float32[3] = 1.0f;

        VkCommandBufferBeginInfo beginInfo = new() { SType = VkStructureType.CommandBufferBeginInfo };
        device.BeginCommandBuffer(commandBuffer, ref beginInfo);

        VkImageMemoryBarrier toTransferDst = new()
        {
            SType = VkStructureType.ImageMemoryBarrier,
            SrcAccessMask = default,
            DstAccessMask = VkAccessFlags.TransferWriteBit,
            OldLayout = VkImageLayout.Undefined,
            NewLayout = VkImageLayout.TransferDstOptimal,
            SrcQueueFamilyIndex = Vulkan.VK_QUEUE_FAMILY_IGNORED,
            DstQueueFamilyIndex = Vulkan.VK_QUEUE_FAMILY_IGNORED,
            Image = swapchainImages[imageIndex],
            SubresourceRange = subresourceRange,
        };
        device.CmdPipelineBarrier(commandBuffer, VkPipelineStageFlags.TopOfPipeBit, VkPipelineStageFlags.TransferBit, default, 0, (VkMemoryBarrier*)null, 0, (VkBufferMemoryBarrier*)null, 1, &toTransferDst);

        device.CmdClearColorImage(commandBuffer, swapchainImages[imageIndex], VkImageLayout.TransferDstOptimal, &clearColor, 1, &subresourceRange);

        VkImageMemoryBarrier toPresent = new()
        {
            SType = VkStructureType.ImageMemoryBarrier,
            SrcAccessMask = VkAccessFlags.TransferWriteBit,
            DstAccessMask = default,
            OldLayout = VkImageLayout.TransferDstOptimal,
            NewLayout = VkImageLayout.PresentSrcKhr,
            SrcQueueFamilyIndex = Vulkan.VK_QUEUE_FAMILY_IGNORED,
            DstQueueFamilyIndex = Vulkan.VK_QUEUE_FAMILY_IGNORED,
            Image = swapchainImages[imageIndex],
            SubresourceRange = subresourceRange,
        };
        device.CmdPipelineBarrier(commandBuffer, VkPipelineStageFlags.TransferBit, VkPipelineStageFlags.BottomOfPipeBit, default, 0, (VkMemoryBarrier*)null, 0, (VkBufferMemoryBarrier*)null, 1, &toPresent);

        device.EndCommandBuffer(commandBuffer);

        VkPipelineStageFlags waitStage = VkPipelineStageFlags.TransferBit;
        VkCommandBuffer cmd = commandBuffer;
        VkSubmitInfo submitInfo = new()
        {
            SType = VkStructureType.SubmitInfo,
            WaitSemaphoreCount = 1,
            PWaitSemaphores = &imageAvailable,
            PWaitDstStageMask = &waitStage,
            CommandBufferCount = 1,
            PCommandBuffers = &cmd,
            SignalSemaphoreCount = 1,
            PSignalSemaphores = &renderFinished,
        };
        device.QueueSubmit(queue, 1, &submitInfo, frameFence);

        VkSwapchainKHR swapchainLocal = swapchain;
        VkPresentInfoKHR presentInfo = new()
        {
            SType = VkStructureType.PresentInfoKhr,
            WaitSemaphoreCount = 1,
            PWaitSemaphores = &renderFinished,
            SwapchainCount = 1,
            PSwapchains = &swapchainLocal,
            PImageIndices = &imageIndex,
        };
        result = swapchainExt.QueuePresentKHR(queue, ref presentInfo);
        if (result != VkResult.Success && result != VkResult.SuboptimalKhr)
        {
            Console.WriteLine($"vkQueuePresentKHR failed: {result}");
            break;
        }
    }

    device.DeviceWaitIdle(rawDevice);
    device.DestroyFence(rawDevice, frameFence, (VkAllocationCallbacks*)null);
    device.DestroySemaphore(rawDevice, renderFinished, (VkAllocationCallbacks*)null);
    device.DestroySemaphore(rawDevice, imageAvailable, (VkAllocationCallbacks*)null);
    device.DestroyCommandPool(rawDevice, commandPool, (VkAllocationCallbacks*)null);
    swapchainExt.DestroySwapchainKHR(rawDevice, swapchain, (VkAllocationCallbacks*)null);
    surfaceExt.DestroySurfaceKHR(rawInstance, surface, (VkAllocationCallbacks*)null);
    device.Dispose();
    instance.Dispose();
}

SDL.DestroyWindow(window);
SDL.Quit();