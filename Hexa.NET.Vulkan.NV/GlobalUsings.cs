// A few NV structs (e.g. ray tracing displacement micromap) reference types owned by
// Hexa.NET.Vulkan.EXT (VkMicromapEXT, VkMicromapUsageEXT). Sibling namespaces aren't
// automatically visible to each other the way a namespace sees its enclosing "Hexa.NET.Vulkan",
// so this project needs an explicit using for them.
global using Hexa.NET.Vulkan.EXT;
global using Hexa.NET.Vulkan.KHR;
