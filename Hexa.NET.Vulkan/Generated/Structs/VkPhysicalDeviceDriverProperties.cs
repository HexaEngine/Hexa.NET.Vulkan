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
	public unsafe struct VkPhysicalDeviceDriverProperties
	{
		public VkStructureType SType;
		public void* PNext;
		public VkDriverId DriverID;
		public fixed byte DriverName[256];
		public fixed byte DriverInfo[256];
		public VkConformanceVersion ConformanceVersion;
	}
}
