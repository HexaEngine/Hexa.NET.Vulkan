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
	public unsafe struct VkFramebufferCreateInfo
	{
		public VkStructureType SType;
		public void* PNext;
		public VkFramebufferCreateFlags Flags;
		public VkRenderPass RenderPass;
		public uint AttachmentCount;
		public VkImageView* PAttachments;
		public uint Width;
		public uint Height;
		public uint Layers;
	}
}