namespace Generator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GLRegistryExtensions
    {
        public static void GatherUsedTargets(this VkRegistry registry, string featureTarget, bool compatibility, out HashSet<string> usedEnums, out HashSet<string> usedCommands)
        {
            bool es = false;
            if (featureTarget.Contains("GL_VERSION_ES"))
            {
                es = true;
            }
            usedEnums = [];
            usedCommands = [];
            foreach (var feature in registry.Features)
            {
                if (es && !feature.Name.Contains("GL_VERSION_ES"))
                {
                    continue;
                }

                foreach (var require in feature.Require)
                {
                    if (require.Profile != null)
                    {
                        if (require.Profile == "core" && compatibility)
                        {
                            continue;
                        }

                        if (require.Profile == "compatibility" && !compatibility)
                        {
                            continue;
                        }
                    }

                    foreach (var @enum in require.Enums)
                    {
                        usedEnums.Add(@enum.Name);
                    }

                    foreach (var command in require.Commands)
                    {
                        usedCommands.Add(command.Name);
                    }
                }

                foreach (var remove in feature.Remove)
                {
                    if (remove.Profile != null)
                    {
                        if (remove.Profile == "core" && compatibility)
                        {
                            continue;
                        }

                        if (remove.Profile == "compatibility" && !compatibility)
                        {
                            continue;
                        }
                    }

                    foreach (var @enum in remove.Enums)
                    {
                        usedEnums.Remove(@enum.Name);
                    }

                    foreach (var command in remove.Commands)
                    {
                        usedCommands.Remove(command.Name);
                    }
                }

                if (feature.Name == featureTarget)
                {
                    break;
                }
            }
        }

        public static IEnumerable<(string extensionName, string originalName, HashSet<string> usedEnums, HashSet<string> usedCommands)> GatherUsedTargetsForExtension(this VkRegistry registry, string featureTarget, HashSet<string> apiTargets)
        {
            HashSet<string> androidTarget = [
                "GL_KHR_debug",
            "GL_KHR_texture_compression_astc_ldr",
            "GL_KHR_blend_equation_advanced",
            "GL_OES_sample_shading",
            "GL_OES_sample_variables",
            "GL_OES_shader_image_atomic",
            "GL_OES_shader_multisample_interpolation",
            "GL_OES_texture_stencil8",
            "GL_OES_texture_storage_multisample_2d_array",
            "GL_EXT_copy_image",
            "GL_EXT_draw_buffers_indexed",
            "GL_EXT_geometry_shader",
            "GL_EXT_gpu_shader5",
            "GL_EXT_primitive_bounding_box",
            "GL_EXT_shader_io_blocks",
            "GL_EXT_tessellation_shader",
            "GL_EXT_texture_border_clamp",
            "GL_EXT_texture_buffer",
            "GL_EXT_texture_cube_map_array",
            "GL_EXT_texture_srgb_decode",
            ];
            HashSet<string> usedEnums = [];
            HashSet<string> usedCommands = [];
            foreach (var extension in registry.Extensions.Extension)
            {
                usedEnums.Clear();
                usedCommands.Clear();

                if (featureTarget == "GL_ANDROID")
                {
                    if (!androidTarget.Contains(extension.Name))
                    {
                        continue;
                    }
                }
                else if (!extension.Name.StartsWith(featureTarget))
                {
                    continue;
                }

                if (!extension.SupportedList.Any(apiTargets.Contains))
                {
                    continue;
                }

                string nameFormatted = string.Join(string.Empty, extension.Name.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Capitalize));

                foreach (var require in extension.Require)
                {
                    foreach (var @enum in require.Enums)
                    {
                        usedEnums.Add(@enum.Name);
                    }

                    foreach (var command in require.Commands)
                    {
                        usedCommands.Add(command.Name);
                    }
                }

                yield return (nameFormatted, extension.Name, usedEnums, usedCommands);
            }
        }

        public static unsafe string Capitalize(this string x)
        {
            if (x.Length == 0) return x;

            fixed (char* px = x)
            {
                px[0] = char.ToUpper(px[0]);
            }

            return x;
        }

        public static unsafe string CapitalizeCopy(this string input)
        {
            string result = new(input);

            fixed (char* pOut = result)
            {
                *pOut = char.ToUpper(*pOut);
            }

            return result;
        }
    }
}