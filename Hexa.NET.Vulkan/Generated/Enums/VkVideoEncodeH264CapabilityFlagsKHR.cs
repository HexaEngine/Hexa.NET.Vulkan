// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using HexaGen.Runtime;
using System.Numerics;

namespace Hexa.NET.Vulkan
{
	public enum VkVideoEncodeH264CapabilityFlagsKHR : uint
	{
		/// <remarks></remarks>
		HrdComplianceBitKhr = unchecked((uint)1),

		/// <remarks></remarks>
		PredictionWeightTableGeneratedBitKhr = unchecked((uint)2),

		/// <remarks></remarks>
		RowUnalignedSliceBitKhr = unchecked((uint)4),

		/// <remarks></remarks>
		DifferentSliceTypeBitKhr = unchecked((uint)8),

		/// <remarks></remarks>
		BFrameInL0ListBitKhr = unchecked((uint)16),

		/// <remarks></remarks>
		BFrameInL1ListBitKhr = unchecked((uint)32),

		/// <remarks></remarks>
		PerPictureTypeMinMaxQpBitKhr = unchecked((uint)64),

		/// <remarks></remarks>
		PerSliceConstantQpBitKhr = unchecked((uint)128),

		/// <remarks></remarks>
		GeneratePrefixNaluBitKhr = unchecked((uint)256),
	}
}
