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
	public unsafe struct VkSubpassDescription2
	{
		public VkStructureType SType;
		public void* PNext;
		public VkSubpassDescriptionFlags Flags;
		public VkPipelineBindPoint PipelineBindPoint;
		public uint ViewMask;
		public uint InputAttachmentCount;
		public VkAttachmentReference2* PInputAttachments;
		public uint ColorAttachmentCount;
		public VkAttachmentReference2* PColorAttachments;
		public VkAttachmentReference2* PResolveAttachments;
		public VkAttachmentReference2* PDepthStencilAttachment;
		public uint PreserveAttachmentCount;
		public uint* PPreserveAttachments;
	}
}
