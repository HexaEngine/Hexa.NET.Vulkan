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
	public unsafe struct VkSamplerCreateInfo
	{
		public VkStructureType SType;
		public void* PNext;
		public VkSamplerCreateFlags Flags;
		public VkFilter MagFilter;
		public VkFilter MinFilter;
		public VkSamplerMipmapMode MipmapMode;
		public VkSamplerAddressMode AddressModeU;
		public VkSamplerAddressMode AddressModeV;
		public VkSamplerAddressMode AddressModeW;
		public float MipLodBias;
		public VkBool32 AnisotropyEnable;
		public float MaxAnisotropy;
		public VkBool32 CompareEnable;
		public VkCompareOp CompareOp;
		public float MinLod;
		public float MaxLod;
		public VkBorderColor BorderColor;
		public VkBool32 UnnormalizedCoordinates;
	}
}