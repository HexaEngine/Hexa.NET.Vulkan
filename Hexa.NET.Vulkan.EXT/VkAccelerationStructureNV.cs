namespace Hexa.NET.Vulkan.EXT
{
    using System;
    using System.Diagnostics;

    // VkAccelerationStructureCaptureDescriptorDataInfoEXT has an optional VkAccelerationStructureNV
    // field for backward compatibility with the original NV ray tracing extension. That type is
    // otherwise owned by Hexa.NET.Vulkan.NV, which itself depends on this (EXT) project, so it is
    // vendored here instead of adding a circular project reference.
#if NET5_0_OR_GREATER
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
#endif
    public readonly partial struct VkAccelerationStructureNV : IEquatable<VkAccelerationStructureNV>
    {
        public VkAccelerationStructureNV(nint handle) { Handle = handle; }
        public nint Handle { get; }
        public bool IsNull => Handle == 0;
        public static VkAccelerationStructureNV Null => new VkAccelerationStructureNV(0);
        public static implicit operator VkAccelerationStructureNV(nint handle) => new VkAccelerationStructureNV(handle);
        public static bool operator ==(VkAccelerationStructureNV left, VkAccelerationStructureNV right) => left.Handle == right.Handle;
        public static bool operator !=(VkAccelerationStructureNV left, VkAccelerationStructureNV right) => left.Handle != right.Handle;
        public static bool operator ==(VkAccelerationStructureNV left, nint right) => left.Handle == right;
        public static bool operator !=(VkAccelerationStructureNV left, nint right) => left.Handle != right;
        public bool Equals(VkAccelerationStructureNV other) => Handle == other.Handle;
        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is VkAccelerationStructureNV handle && Equals(handle);
        /// <inheritdoc/>
        public override int GetHashCode() => Handle.GetHashCode();
#if NET5_0_OR_GREATER
        private string DebuggerDisplay => string.Format("VkAccelerationStructureNV [0x{0}]", Handle.ToString("X"));
#endif
    }
}
