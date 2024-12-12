namespace Hexa.NET.Vulkan
{
    public struct VkBool32 : IEquatable<VkBool32>
    {
        public int Value;

        public VkBool32(int value)
        {
            Value = value;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is VkBool32 @bool && Equals(@bool);
        }

        public readonly bool Equals(VkBool32 other)
        {
            return Value == other.Value;
        }

        public override readonly int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(VkBool32 left, VkBool32 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VkBool32 left, VkBool32 right)
        {
            return !(left == right);
        }

        public static implicit operator VkBool32(bool value) => new VkBool32(value ? 1 : 0);

        public static implicit operator bool(VkBool32 value) => value.Value == 1;
    }

    public struct VkDeviceSize
    {
        public ulong Value;
    }

    public struct VkDeviceAddress
    {
        public ulong Value;
    }
}