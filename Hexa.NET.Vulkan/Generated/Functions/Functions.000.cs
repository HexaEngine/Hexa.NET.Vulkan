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
using System.Runtime.CompilerServices;
using System.Numerics;

namespace Hexa.NET.Vulkan
{
	public static unsafe partial class Vulkan
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static VkResult CreateInstanceNative(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance)
		{
			#if NET5_0_OR_GREATER
			return ((delegate* unmanaged[Cdecl]<VkInstanceCreateInfo*, VkAllocationCallbacks*, VkInstance*, VkResult>)funcTable[0])(pCreateInfo, pAllocator, pInstance);
			#else
			return (VkResult)((delegate* unmanaged[Cdecl]<nint, nint, nint, VkResult>)funcTable[0])((nint)pCreateInfo, (nint)pAllocator, (nint)pInstance);
			#endif
		}

		public static VkResult CreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance)
		{
			VkResult ret = CreateInstanceNative(pCreateInfo, pAllocator, pInstance);
			return ret;
		}

		public static VkResult CreateInstance(Span<VkInstanceCreateInfo> pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance)
		{
			fixed (VkInstanceCreateInfo* ppCreateInfo0 = pCreateInfo)
			{
				VkResult ret = CreateInstanceNative(ppCreateInfo0, pAllocator, pInstance);
				return ret;
			}
		}

		public static VkResult CreateInstance(ref VkInstanceCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance)
		{
			fixed (VkInstanceCreateInfo* ppCreateInfo0 = &pCreateInfo)
			{
				VkResult ret = CreateInstanceNative(ppCreateInfo0, pAllocator, pInstance);
				return ret;
			}
		}

		public static VkResult CreateInstance(VkInstanceCreateInfo* pCreateInfo, Span<VkAllocationCallbacks> pAllocator, VkInstance* pInstance)
		{
			fixed (VkAllocationCallbacks* ppAllocator0 = pAllocator)
			{
				VkResult ret = CreateInstanceNative(pCreateInfo, ppAllocator0, pInstance);
				return ret;
			}
		}

		public static VkResult CreateInstance(VkInstanceCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkInstance* pInstance)
		{
			fixed (VkAllocationCallbacks* ppAllocator0 = &pAllocator)
			{
				VkResult ret = CreateInstanceNative(pCreateInfo, ppAllocator0, pInstance);
				return ret;
			}
		}

		public static VkResult CreateInstance(Span<VkInstanceCreateInfo> pCreateInfo, Span<VkAllocationCallbacks> pAllocator, VkInstance* pInstance)
		{
			fixed (VkInstanceCreateInfo* ppCreateInfo0 = pCreateInfo)
			{
				fixed (VkAllocationCallbacks* ppAllocator1 = pAllocator)
				{
					VkResult ret = CreateInstanceNative(ppCreateInfo0, ppAllocator1, pInstance);
					return ret;
				}
			}
		}

		public static VkResult CreateInstance(ref VkInstanceCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkInstance* pInstance)
		{
			fixed (VkInstanceCreateInfo* ppCreateInfo0 = &pCreateInfo)
			{
				fixed (VkAllocationCallbacks* ppAllocator1 = &pAllocator)
				{
					VkResult ret = CreateInstanceNative(ppCreateInfo0, ppAllocator1, pInstance);
					return ret;
				}
			}
		}

		public static VkResult CreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, Span<VkInstance> pInstance)
		{
			fixed (VkInstance* ppInstance0 = pInstance)
			{
				VkResult ret = CreateInstanceNative(pCreateInfo, pAllocator, ppInstance0);
				return ret;
			}
		}

		public static VkResult CreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, ref VkInstance pInstance)
		{
			fixed (VkInstance* ppInstance0 = &pInstance)
			{
				VkResult ret = CreateInstanceNative(pCreateInfo, pAllocator, ppInstance0);
				return ret;
			}
		}

		public static VkResult CreateInstance(Span<VkInstanceCreateInfo> pCreateInfo, VkAllocationCallbacks* pAllocator, Span<VkInstance> pInstance)
		{
			fixed (VkInstanceCreateInfo* ppCreateInfo0 = pCreateInfo)
			{
				fixed (VkInstance* ppInstance1 = pInstance)
				{
					VkResult ret = CreateInstanceNative(ppCreateInfo0, pAllocator, ppInstance1);
					return ret;
				}
			}
		}

		public static VkResult CreateInstance(ref VkInstanceCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, ref VkInstance pInstance)
		{
			fixed (VkInstanceCreateInfo* ppCreateInfo0 = &pCreateInfo)
			{
				fixed (VkInstance* ppInstance1 = &pInstance)
				{
					VkResult ret = CreateInstanceNative(ppCreateInfo0, pAllocator, ppInstance1);
					return ret;
				}
			}
		}

		public static VkResult CreateInstance(VkInstanceCreateInfo* pCreateInfo, Span<VkAllocationCallbacks> pAllocator, Span<VkInstance> pInstance)
		{
			fixed (VkAllocationCallbacks* ppAllocator0 = pAllocator)
			{
				fixed (VkInstance* ppInstance1 = pInstance)
				{
					VkResult ret = CreateInstanceNative(pCreateInfo, ppAllocator0, ppInstance1);
					return ret;
				}
			}
		}

		public static VkResult CreateInstance(VkInstanceCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, ref VkInstance pInstance)
		{
			fixed (VkAllocationCallbacks* ppAllocator0 = &pAllocator)
			{
				fixed (VkInstance* ppInstance1 = &pInstance)
				{
					VkResult ret = CreateInstanceNative(pCreateInfo, ppAllocator0, ppInstance1);
					return ret;
				}
			}
		}

		public static VkResult CreateInstance(Span<VkInstanceCreateInfo> pCreateInfo, Span<VkAllocationCallbacks> pAllocator, Span<VkInstance> pInstance)
		{
			fixed (VkInstanceCreateInfo* ppCreateInfo0 = pCreateInfo)
			{
				fixed (VkAllocationCallbacks* ppAllocator1 = pAllocator)
				{
					fixed (VkInstance* ppInstance2 = pInstance)
					{
						VkResult ret = CreateInstanceNative(ppCreateInfo0, ppAllocator1, ppInstance2);
						return ret;
					}
				}
			}
		}

		public static VkResult CreateInstance(ref VkInstanceCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, ref VkInstance pInstance)
		{
			fixed (VkInstanceCreateInfo* ppCreateInfo0 = &pCreateInfo)
			{
				fixed (VkAllocationCallbacks* ppAllocator1 = &pAllocator)
				{
					fixed (VkInstance* ppInstance2 = &pInstance)
					{
						VkResult ret = CreateInstanceNative(ppCreateInfo0, ppAllocator1, ppInstance2);
						return ret;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void DestroyInstanceNative(VkInstance instance, VkAllocationCallbacks* pAllocator)
		{
			#if NET5_0_OR_GREATER
			((delegate* unmanaged[Cdecl]<VkInstance, VkAllocationCallbacks*, void>)funcTable[1])(instance, pAllocator);
			#else
			((delegate* unmanaged[Cdecl]<VkInstance, nint, void>)funcTable[1])(instance, (nint)pAllocator);
			#endif
		}

		public static void DestroyInstance(VkInstance instance, VkAllocationCallbacks* pAllocator)
		{
			DestroyInstanceNative(instance, pAllocator);
		}

		public static void DestroyInstance(VkInstance instance, Span<VkAllocationCallbacks> pAllocator)
		{
			fixed (VkAllocationCallbacks* ppAllocator0 = pAllocator)
			{
				DestroyInstanceNative(instance, ppAllocator0);
			}
		}

		public static void DestroyInstance(VkInstance instance, ref VkAllocationCallbacks pAllocator)
		{
			fixed (VkAllocationCallbacks* ppAllocator0 = &pAllocator)
			{
				DestroyInstanceNative(instance, ppAllocator0);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static delegate*unmanaged[Cdecl]<void> GetDeviceProcAddrNative(VkDevice device, byte* pName)
		{
			#if NET5_0_OR_GREATER
			return ((delegate* unmanaged[Cdecl]<VkDevice, byte*, delegate*unmanaged[Cdecl]<void>>)funcTable[2])(device, pName);
			#else
			return (delegate*unmanaged[Cdecl]<void>)((delegate* unmanaged[Cdecl]<VkDevice, nint, nint>)funcTable[2])(device, (nint)pName);
			#endif
		}

		public static delegate*unmanaged[Cdecl]<void> GetDeviceProcAddr(VkDevice device, byte* pName)
		{
			delegate*unmanaged[Cdecl]<void> ret = GetDeviceProcAddrNative(device, pName);
			return ret;
		}

		public static delegate*unmanaged[Cdecl]<void> GetDeviceProcAddr(VkDevice device, string pName)
		{
			byte* pStr0 = null;
			int pStrSize0 = 0;
			if (pName != null)
			{
				pStrSize0 = Utils.GetByteCountUTF8(pName);
				if (pStrSize0 >= Utils.MaxStackallocSize)
				{
					pStr0 = Utils.Alloc<byte>(pStrSize0 + 1);
				}
				else
				{
					byte* pStrStack0 = stackalloc byte[pStrSize0 + 1];
					pStr0 = pStrStack0;
				}
				int pStrOffset0 = Utils.EncodeStringUTF8(pName, pStr0, pStrSize0);
				pStr0[pStrOffset0] = 0;
			}
			delegate*unmanaged[Cdecl]<void> ret = GetDeviceProcAddrNative(device, pStr0);
			if (pStrSize0 >= Utils.MaxStackallocSize)
			{
				Utils.Free(pStr0);
			}
			return ret;
		}

		public static delegate*unmanaged[Cdecl]<void> GetDeviceProcAddr(VkDevice device, Span<byte> pName)
		{
			fixed (byte* ppName0 = pName)
			{
				delegate*unmanaged[Cdecl]<void> ret = GetDeviceProcAddrNative(device, ppName0);
				return ret;
			}
		}

		public static delegate*unmanaged[Cdecl]<void> GetDeviceProcAddr(VkDevice device, ref byte pName)
		{
			fixed (byte* ppName0 = &pName)
			{
				delegate*unmanaged[Cdecl]<void> ret = GetDeviceProcAddrNative(device, ppName0);
				return ret;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static delegate*unmanaged[Cdecl]<void> GetInstanceProcAddrNative(VkInstance instance, byte* pName)
		{
			#if NET5_0_OR_GREATER
			return ((delegate* unmanaged[Cdecl]<VkInstance, byte*, delegate*unmanaged[Cdecl]<void>>)funcTable[3])(instance, pName);
			#else
			return (delegate*unmanaged[Cdecl]<void>)((delegate* unmanaged[Cdecl]<VkInstance, nint, nint>)funcTable[3])(instance, (nint)pName);
			#endif
		}

		public static delegate*unmanaged[Cdecl]<void> GetInstanceProcAddr(VkInstance instance, byte* pName)
		{
			delegate*unmanaged[Cdecl]<void> ret = GetInstanceProcAddrNative(instance, pName);
			return ret;
		}

		public static delegate*unmanaged[Cdecl]<void> GetInstanceProcAddr(VkInstance instance, string pName)
		{
			byte* pStr0 = null;
			int pStrSize0 = 0;
			if (pName != null)
			{
				pStrSize0 = Utils.GetByteCountUTF8(pName);
				if (pStrSize0 >= Utils.MaxStackallocSize)
				{
					pStr0 = Utils.Alloc<byte>(pStrSize0 + 1);
				}
				else
				{
					byte* pStrStack0 = stackalloc byte[pStrSize0 + 1];
					pStr0 = pStrStack0;
				}
				int pStrOffset0 = Utils.EncodeStringUTF8(pName, pStr0, pStrSize0);
				pStr0[pStrOffset0] = 0;
			}
			delegate*unmanaged[Cdecl]<void> ret = GetInstanceProcAddrNative(instance, pStr0);
			if (pStrSize0 >= Utils.MaxStackallocSize)
			{
				Utils.Free(pStr0);
			}
			return ret;
		}

		public static delegate*unmanaged[Cdecl]<void> GetInstanceProcAddr(VkInstance instance, Span<byte> pName)
		{
			fixed (byte* ppName0 = pName)
			{
				delegate*unmanaged[Cdecl]<void> ret = GetInstanceProcAddrNative(instance, ppName0);
				return ret;
			}
		}

		public static delegate*unmanaged[Cdecl]<void> GetInstanceProcAddr(VkInstance instance, ref byte pName)
		{
			fixed (byte* ppName0 = &pName)
			{
				delegate*unmanaged[Cdecl]<void> ret = GetInstanceProcAddrNative(instance, ppName0);
				return ret;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void DestroyDeviceNative(VkDevice device, VkAllocationCallbacks* pAllocator)
		{
			#if NET5_0_OR_GREATER
			((delegate* unmanaged[Cdecl]<VkDevice, VkAllocationCallbacks*, void>)funcTable[4])(device, pAllocator);
			#else
			((delegate* unmanaged[Cdecl]<VkDevice, nint, void>)funcTable[4])(device, (nint)pAllocator);
			#endif
		}

		public static void DestroyDevice(VkDevice device, VkAllocationCallbacks* pAllocator)
		{
			DestroyDeviceNative(device, pAllocator);
		}

		public static void DestroyDevice(VkDevice device, Span<VkAllocationCallbacks> pAllocator)
		{
			fixed (VkAllocationCallbacks* ppAllocator0 = pAllocator)
			{
				DestroyDeviceNative(device, ppAllocator0);
			}
		}

		public static void DestroyDevice(VkDevice device, ref VkAllocationCallbacks pAllocator)
		{
			fixed (VkAllocationCallbacks* ppAllocator0 = &pAllocator)
			{
				DestroyDeviceNative(device, ppAllocator0);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static VkResult EnumerateInstanceVersionNative(uint* pApiVersion)
		{
			#if NET5_0_OR_GREATER
			return ((delegate* unmanaged[Cdecl]<uint*, VkResult>)funcTable[5])(pApiVersion);
			#else
			return (VkResult)((delegate* unmanaged[Cdecl]<nint, VkResult>)funcTable[5])((nint)pApiVersion);
			#endif
		}

		public static VkResult EnumerateInstanceVersion(uint* pApiVersion)
		{
			VkResult ret = EnumerateInstanceVersionNative(pApiVersion);
			return ret;
		}

		public static VkResult EnumerateInstanceVersion(Span<uint> pApiVersion)
		{
			fixed (uint* ppApiVersion0 = pApiVersion)
			{
				VkResult ret = EnumerateInstanceVersionNative(ppApiVersion0);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceVersion(ref uint pApiVersion)
		{
			fixed (uint* ppApiVersion0 = &pApiVersion)
			{
				VkResult ret = EnumerateInstanceVersionNative(ppApiVersion0);
				return ret;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static VkResult EnumerateInstanceLayerPropertiesNative(uint* pPropertyCount, VkLayerProperties* pProperties)
		{
			#if NET5_0_OR_GREATER
			return ((delegate* unmanaged[Cdecl]<uint*, VkLayerProperties*, VkResult>)funcTable[6])(pPropertyCount, pProperties);
			#else
			return (VkResult)((delegate* unmanaged[Cdecl]<nint, nint, VkResult>)funcTable[6])((nint)pPropertyCount, (nint)pProperties);
			#endif
		}

		public static VkResult EnumerateInstanceLayerProperties(uint* pPropertyCount, VkLayerProperties* pProperties)
		{
			VkResult ret = EnumerateInstanceLayerPropertiesNative(pPropertyCount, pProperties);
			return ret;
		}

		public static VkResult EnumerateInstanceLayerProperties(Span<uint> pPropertyCount, VkLayerProperties* pProperties)
		{
			fixed (uint* ppPropertyCount0 = pPropertyCount)
			{
				VkResult ret = EnumerateInstanceLayerPropertiesNative(ppPropertyCount0, pProperties);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceLayerProperties(ref uint pPropertyCount, VkLayerProperties* pProperties)
		{
			fixed (uint* ppPropertyCount0 = &pPropertyCount)
			{
				VkResult ret = EnumerateInstanceLayerPropertiesNative(ppPropertyCount0, pProperties);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceLayerProperties(uint* pPropertyCount, Span<VkLayerProperties> pProperties)
		{
			fixed (VkLayerProperties* ppProperties0 = pProperties)
			{
				VkResult ret = EnumerateInstanceLayerPropertiesNative(pPropertyCount, ppProperties0);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceLayerProperties(uint* pPropertyCount, ref VkLayerProperties pProperties)
		{
			fixed (VkLayerProperties* ppProperties0 = &pProperties)
			{
				VkResult ret = EnumerateInstanceLayerPropertiesNative(pPropertyCount, ppProperties0);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceLayerProperties(Span<uint> pPropertyCount, Span<VkLayerProperties> pProperties)
		{
			fixed (uint* ppPropertyCount0 = pPropertyCount)
			{
				fixed (VkLayerProperties* ppProperties1 = pProperties)
				{
					VkResult ret = EnumerateInstanceLayerPropertiesNative(ppPropertyCount0, ppProperties1);
					return ret;
				}
			}
		}

		public static VkResult EnumerateInstanceLayerProperties(ref uint pPropertyCount, ref VkLayerProperties pProperties)
		{
			fixed (uint* ppPropertyCount0 = &pPropertyCount)
			{
				fixed (VkLayerProperties* ppProperties1 = &pProperties)
				{
					VkResult ret = EnumerateInstanceLayerPropertiesNative(ppPropertyCount0, ppProperties1);
					return ret;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static VkResult EnumerateInstanceExtensionPropertiesNative(byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties)
		{
			#if NET5_0_OR_GREATER
			return ((delegate* unmanaged[Cdecl]<byte*, uint*, VkExtensionProperties*, VkResult>)funcTable[7])(pLayerName, pPropertyCount, pProperties);
			#else
			return (VkResult)((delegate* unmanaged[Cdecl]<nint, nint, nint, VkResult>)funcTable[7])((nint)pLayerName, (nint)pPropertyCount, (nint)pProperties);
			#endif
		}

		public static VkResult EnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties)
		{
			VkResult ret = EnumerateInstanceExtensionPropertiesNative(pLayerName, pPropertyCount, pProperties);
			return ret;
		}

		public static VkResult EnumerateInstanceExtensionProperties(string pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties)
		{
			byte* pStr0 = null;
			int pStrSize0 = 0;
			if (pLayerName != null)
			{
				pStrSize0 = Utils.GetByteCountUTF8(pLayerName);
				if (pStrSize0 >= Utils.MaxStackallocSize)
				{
					pStr0 = Utils.Alloc<byte>(pStrSize0 + 1);
				}
				else
				{
					byte* pStrStack0 = stackalloc byte[pStrSize0 + 1];
					pStr0 = pStrStack0;
				}
				int pStrOffset0 = Utils.EncodeStringUTF8(pLayerName, pStr0, pStrSize0);
				pStr0[pStrOffset0] = 0;
			}
			VkResult ret = EnumerateInstanceExtensionPropertiesNative(pStr0, pPropertyCount, pProperties);
			if (pStrSize0 >= Utils.MaxStackallocSize)
			{
				Utils.Free(pStr0);
			}
			return ret;
		}

		public static VkResult EnumerateInstanceExtensionProperties(Span<byte> pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties)
		{
			fixed (byte* ppLayerName0 = pLayerName)
			{
				VkResult ret = EnumerateInstanceExtensionPropertiesNative(ppLayerName0, pPropertyCount, pProperties);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(ref byte pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties)
		{
			fixed (byte* ppLayerName0 = &pLayerName)
			{
				VkResult ret = EnumerateInstanceExtensionPropertiesNative(ppLayerName0, pPropertyCount, pProperties);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(byte* pLayerName, Span<uint> pPropertyCount, VkExtensionProperties* pProperties)
		{
			fixed (uint* ppPropertyCount0 = pPropertyCount)
			{
				VkResult ret = EnumerateInstanceExtensionPropertiesNative(pLayerName, ppPropertyCount0, pProperties);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(byte* pLayerName, ref uint pPropertyCount, VkExtensionProperties* pProperties)
		{
			fixed (uint* ppPropertyCount0 = &pPropertyCount)
			{
				VkResult ret = EnumerateInstanceExtensionPropertiesNative(pLayerName, ppPropertyCount0, pProperties);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(Span<byte> pLayerName, Span<uint> pPropertyCount, VkExtensionProperties* pProperties)
		{
			fixed (byte* ppLayerName0 = pLayerName)
			{
				fixed (uint* ppPropertyCount1 = pPropertyCount)
				{
					VkResult ret = EnumerateInstanceExtensionPropertiesNative(ppLayerName0, ppPropertyCount1, pProperties);
					return ret;
				}
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(ref byte pLayerName, ref uint pPropertyCount, VkExtensionProperties* pProperties)
		{
			fixed (byte* ppLayerName0 = &pLayerName)
			{
				fixed (uint* ppPropertyCount1 = &pPropertyCount)
				{
					VkResult ret = EnumerateInstanceExtensionPropertiesNative(ppLayerName0, ppPropertyCount1, pProperties);
					return ret;
				}
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, Span<VkExtensionProperties> pProperties)
		{
			fixed (VkExtensionProperties* ppProperties0 = pProperties)
			{
				VkResult ret = EnumerateInstanceExtensionPropertiesNative(pLayerName, pPropertyCount, ppProperties0);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, ref VkExtensionProperties pProperties)
		{
			fixed (VkExtensionProperties* ppProperties0 = &pProperties)
			{
				VkResult ret = EnumerateInstanceExtensionPropertiesNative(pLayerName, pPropertyCount, ppProperties0);
				return ret;
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(Span<byte> pLayerName, uint* pPropertyCount, Span<VkExtensionProperties> pProperties)
		{
			fixed (byte* ppLayerName0 = pLayerName)
			{
				fixed (VkExtensionProperties* ppProperties1 = pProperties)
				{
					VkResult ret = EnumerateInstanceExtensionPropertiesNative(ppLayerName0, pPropertyCount, ppProperties1);
					return ret;
				}
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(ref byte pLayerName, uint* pPropertyCount, ref VkExtensionProperties pProperties)
		{
			fixed (byte* ppLayerName0 = &pLayerName)
			{
				fixed (VkExtensionProperties* ppProperties1 = &pProperties)
				{
					VkResult ret = EnumerateInstanceExtensionPropertiesNative(ppLayerName0, pPropertyCount, ppProperties1);
					return ret;
				}
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(byte* pLayerName, Span<uint> pPropertyCount, Span<VkExtensionProperties> pProperties)
		{
			fixed (uint* ppPropertyCount0 = pPropertyCount)
			{
				fixed (VkExtensionProperties* ppProperties1 = pProperties)
				{
					VkResult ret = EnumerateInstanceExtensionPropertiesNative(pLayerName, ppPropertyCount0, ppProperties1);
					return ret;
				}
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(byte* pLayerName, ref uint pPropertyCount, ref VkExtensionProperties pProperties)
		{
			fixed (uint* ppPropertyCount0 = &pPropertyCount)
			{
				fixed (VkExtensionProperties* ppProperties1 = &pProperties)
				{
					VkResult ret = EnumerateInstanceExtensionPropertiesNative(pLayerName, ppPropertyCount0, ppProperties1);
					return ret;
				}
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(Span<byte> pLayerName, Span<uint> pPropertyCount, Span<VkExtensionProperties> pProperties)
		{
			fixed (byte* ppLayerName0 = pLayerName)
			{
				fixed (uint* ppPropertyCount1 = pPropertyCount)
				{
					fixed (VkExtensionProperties* ppProperties2 = pProperties)
					{
						VkResult ret = EnumerateInstanceExtensionPropertiesNative(ppLayerName0, ppPropertyCount1, ppProperties2);
						return ret;
					}
				}
			}
		}

		public static VkResult EnumerateInstanceExtensionProperties(ref byte pLayerName, ref uint pPropertyCount, ref VkExtensionProperties pProperties)
		{
			fixed (byte* ppLayerName0 = &pLayerName)
			{
				fixed (uint* ppPropertyCount1 = &pPropertyCount)
				{
					fixed (VkExtensionProperties* ppProperties2 = &pProperties)
					{
						VkResult ret = EnumerateInstanceExtensionPropertiesNative(ppLayerName0, ppPropertyCount1, ppProperties2);
						return ret;
					}
				}
			}
		}

	}
}
