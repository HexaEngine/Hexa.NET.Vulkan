namespace Generator
{
    using System.Collections.Generic;

    public interface IVersionedMember
    {
        HashSet<string> SupportedVersions { get; }
        string? SupportedVersionText { get; set; }
    }
}