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
	public unsafe struct VkMemoryToImageCopy
	{
		public VkStructureType SType;
		public void* PNext;
		public void* PHostPointer;
		public uint MemoryRowLength;
		public uint MemoryImageHeight;
		public VkImageSubresourceLayers ImageSubresource;
		public VkOffset3D ImageOffset;
		public VkExtent3D ImageExtent;
	}
}
