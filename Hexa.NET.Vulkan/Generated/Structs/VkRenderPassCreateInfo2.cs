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
	public unsafe struct VkRenderPassCreateInfo2
	{
		public VkStructureType SType;
		public void* PNext;
		public VkRenderPassCreateFlags Flags;
		public uint AttachmentCount;
		public VkAttachmentDescription2* PAttachments;
		public uint SubpassCount;
		public VkSubpassDescription2* PSubpasses;
		public uint DependencyCount;
		public VkSubpassDependency2* PDependencies;
		public uint CorrelatedViewMaskCount;
		public uint* PCorrelatedViewMasks;
	}
}
