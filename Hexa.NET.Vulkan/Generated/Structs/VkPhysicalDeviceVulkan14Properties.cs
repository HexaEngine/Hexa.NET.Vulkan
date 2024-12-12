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
	public unsafe struct VkPhysicalDeviceVulkan14Properties
	{
		public VkStructureType SType;
		public void* PNext;
		public uint LineSubPixelPrecisionBits;
		public uint MaxVertexAttribDivisor;
		public VkBool32 SupportsNonZeroFirstInstance;
		public uint MaxPushDescriptors;
		public VkBool32 DynamicRenderingLocalReadDepthStencilAttachments;
		public VkBool32 DynamicRenderingLocalReadMultisampledAttachments;
		public VkBool32 EarlyFragmentMultisampleCoverageAfterSampleCounting;
		public VkBool32 EarlyFragmentSampleMaskTestBeforeSampleCounting;
		public VkBool32 DepthStencilSwizzleOneSupport;
		public VkBool32 PolygonModePointSize;
		public VkBool32 NonStrictSinglePixelWideLinesUseParallelogram;
		public VkBool32 NonStrictWideLinesUseParallelogram;
		public VkBool32 BlockTexelViewCompatibleMultipleLayers;
		public uint MaxCombinedImageSamplerDescriptorCount;
		public VkBool32 FragmentShadingRateClampCombinerInputs;
		public VkPipelineRobustnessBufferBehavior DefaultRobustnessStorageBuffers;
		public VkPipelineRobustnessBufferBehavior DefaultRobustnessUniformBuffers;
		public VkPipelineRobustnessBufferBehavior DefaultRobustnessVertexInputs;
		public VkPipelineRobustnessImageBehavior DefaultRobustnessImages;
		public uint CopySrcLayoutCount;
		public VkImageLayout* PCopySrcLayouts;
		public uint CopyDstLayoutCount;
		public VkImageLayout* PCopyDstLayouts;
		public fixed byte OptimalTilingLayoutUUID[16];
		public VkBool32 IdenticalMemoryTypeRequirements;
	}
}