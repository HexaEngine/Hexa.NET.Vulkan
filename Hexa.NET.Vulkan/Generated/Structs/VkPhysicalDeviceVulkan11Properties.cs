// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HexaGen.Runtime;
using System.Numerics;

namespace Hexa.NET.Vulkan
{
	public unsafe struct VkPhysicalDeviceVulkan11Properties
	{
		public VkStructureType SType;
		public void* PNext;
		public fixed byte DeviceUUID[16];
		public fixed byte DriverUUID[16];
		public fixed byte DeviceLUID[8];
		public uint DeviceNodeMask;
		public VkBool32 DeviceLUIDValid;
		public uint SubgroupSize;
		public VkShaderStageFlags SubgroupSupportedStages;
		public VkSubgroupFeatureFlags SubgroupSupportedOperations;
		public VkBool32 SubgroupQuadOperationsInAllStages;
		public VkPointClippingBehavior PointClippingBehavior;
		public uint MaxMultiviewViewCount;
		public uint MaxMultiviewInstanceIndex;
		public VkBool32 ProtectedNoFault;
		public uint MaxPerSetDescriptors;
		public VkDeviceSize MaxMemoryAllocationSize;
	}
}
