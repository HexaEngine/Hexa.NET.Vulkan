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
	#if NET5_0_OR_GREATER
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	#endif
	public readonly partial struct VkFramebuffer : IEquatable<VkFramebuffer>
	{
		public VkFramebuffer(nint handle) { Handle = handle; }
		public nint Handle { get; }
		public bool IsNull => Handle == 0;
		public static VkFramebuffer Null => new VkFramebuffer(0);
		public static implicit operator VkFramebuffer(nint handle) => new VkFramebuffer(handle);
		public static bool operator ==(VkFramebuffer left, VkFramebuffer right) => left.Handle == right.Handle;
		public static bool operator !=(VkFramebuffer left, VkFramebuffer right) => left.Handle != right.Handle;
		public static bool operator ==(VkFramebuffer left, nint right) => left.Handle == right;
		public static bool operator !=(VkFramebuffer left, nint right) => left.Handle != right;
		public bool Equals(VkFramebuffer other) => Handle == other.Handle;
		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is VkFramebuffer handle && Equals(handle);
		/// <inheritdoc/>
		public override int GetHashCode() => Handle.GetHashCode();
		#if NET5_0_OR_GREATER
		private string DebuggerDisplay => string.Format("VkFramebuffer [0x{0}]", Handle.ToString("X"));
		#endif
	}
}