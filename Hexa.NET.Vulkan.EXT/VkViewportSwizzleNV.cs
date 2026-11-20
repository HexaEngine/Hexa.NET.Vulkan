namespace Hexa.NET.Vulkan.EXT
{
    // VK_EXT_extended_dynamic_state3 and VK_EXT_shader_object expose dynamic-state setters that
    // take VkViewportSwizzleNV (VK_NV_viewport_swizzle). Hexa.NET.Vulkan.NV depends on this (EXT)
    // project (for VkMicromapEXT), so these two NV types are vendored here instead of adding a
    // circular project reference.
    public enum VkViewportCoordinateSwizzleNV : uint
    {
        PositiveXNv = unchecked((uint)0),
        NegativeXNv = unchecked((uint)1),
        PositiveYNv = unchecked((uint)2),
        NegativeYNv = unchecked((uint)3),
        PositiveZNv = unchecked((uint)4),
        NegativeZNv = unchecked((uint)5),
        PositiveWNv = unchecked((uint)6),
        NegativeWNv = unchecked((uint)7),
    }

    public unsafe struct VkViewportSwizzleNV
    {
        public VkViewportCoordinateSwizzleNV X;
        public VkViewportCoordinateSwizzleNV Y;
        public VkViewportCoordinateSwizzleNV Z;
        public VkViewportCoordinateSwizzleNV W;
    }
}
