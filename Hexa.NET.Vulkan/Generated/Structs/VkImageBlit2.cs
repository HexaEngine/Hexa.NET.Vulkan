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
	public unsafe struct VkImageBlit2
	{
		public VkStructureType SType;
		public void* PNext;
		public VkImageSubresourceLayers SrcSubresource;
		public VkOffset3D SrcOffsets_0;
		public VkOffset3D SrcOffsets_1;
		public VkImageSubresourceLayers DstSubresource;
		public VkOffset3D DstOffsets_0;
		public VkOffset3D DstOffsets_1;
	}
}
