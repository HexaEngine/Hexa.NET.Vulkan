namespace Generator
{
    using System.Collections.Generic;

    public class Config
    {
        public static readonly HashSet<string> AcceptedExtensions = ["GL_EXT", "GL_ARB", "GL_NV", "GL_AMD", "GL_APPLE", "GL_INTEL", "GL_KHR", "GL_MESA", "GL_OVR"];
        public static readonly HashSet<string> AcceptedExtensionsEs = ["GL_ANDROID", "GL_EXT", "GL_AMD", "GL_ANGLE", "GL_ARM", "GL_INTEL", "GL_KHR", "GL_MESA", "GL_NV", "GL_OES"];
        public static readonly HashSet<string> DelegateTypes = ["GLDebugProc", "GLDebugProcARB", "GLVulkanProcNV", "GLDebugProcAMD", "GLDebugProcKHR"];
    }
}