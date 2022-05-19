using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Player.Plugin
{

    public static class PluginLoader
    {
        const string PluginFolder = "plugins";

        public static IList<PluginItem> Load()
        {
            var data = new List<PluginItem>();
            var baseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginFolder);
            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
                return data;
            }
            var files = Directory.GetFiles(baseFolder);
            foreach (var file in files)
            {
                var item = LoadPlugin(file, baseFolder);
                if (item != null)
                {
                    data.Add(item);
                }
            }
            return data;
        }

        public static PluginItem? LoadPlugin(string file, string baseFolder)
        {
            var data = LoadPlugin(file);
            if (data != null)
            {
                data.FileName = Path.GetRelativePath(baseFolder, file);
            }
            return data;
        }

        public static PluginItem? LoadPlugin(string file)
        {
            if (Path.GetExtension(file) != ".dll")
            {
                return null;
            }
            var name = AssemblyName.GetAssemblyName(file);
            var loadContext = new PluginLoadContext(file);
            var assembly = loadContext.LoadFromAssemblyName(name);
            PluginItem? data = null;
            if (assembly != null && IsPlugin(assembly))
            {
                data = new PluginItem()
                {
                    Name = name.Name,
                    Version = name.Version.ToString(),
                    FileName = file
                };
                string? val;
                foreach (var item in assembly.CustomAttributes)
                {
                    if (item.AttributeType == typeof(AssemblyProductAttribute))
                    {
                        val = GetAttributeValue(item.ConstructorArguments);
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            data.Name = val;
                        }
                        continue;
                    }
                    if (item.AttributeType == typeof(AssemblyDescriptionAttribute))
                    {
                        val = GetAttributeValue(item.ConstructorArguments);
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            data.Description = val;
                        }
                        continue;
                    }
                    if (item.AttributeType == typeof(AssemblyCompanyAttribute))
                    {
                        val = GetAttributeValue(item.ConstructorArguments);
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            data.Author = val;
                        }
                        continue;
                    }
                }
            }
            loadContext.Unload();
            return data;
        }

        public static bool IsPlugin(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(INetSource).IsAssignableFrom(type))
                {
                    return true;
                }
            }
            return false;
        }

        public static IList<PluginItem> Save(string[] fileNames)
        {
            var data = new List<PluginItem>();
            var baseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginFolder);
            foreach (var file in fileNames)
            {
                var fileName = Path.GetFileName(file);
                var distFile = Path.Combine(baseFolder, fileName);
                var item = LoadPlugin(file);
                if (item != null)
                {
                    File.Copy(file, distFile, true);
                    item.FileName = fileName;
                    data.Add(item);
                    continue;
                }
            }
            return data;
        }

        private static Assembly LoadAssembly(string path)
        {
            var pluginLocation = Path.GetFullPath(path);
            var loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(pluginLocation));
        }

        private static string? GetAttributeValue(Assembly assembly, Type type)
        {
            return GetAttributeValue(assembly.CustomAttributes, type);
        }

        private static string? GetAttributeValue(IEnumerable<CustomAttributeData> attributes, 
            Type type)
        {
            foreach (var item in attributes)
            {
                if (item.AttributeType == type)
                {
                    return GetAttributeValue(item.ConstructorArguments);
                }
            }
            return null;
        }

        private static string? GetAttributeValue(IList<CustomAttributeTypedArgument> values)
        {
            foreach (var item in values)
            {
                return GetAttributeValue(item);
            }
            return null;
        }

        private static string? GetAttributeValue(CustomAttributeTypedArgument data)
        {
            if (data.Value == null)
            {
                return null;
            }
            if (data.Value.GetType() != typeof(ReadOnlyCollection<CustomAttributeTypedArgument>))
            {
                return data.Value.ToString();
            }
            var sb = new StringBuilder();
            foreach (var item in (ReadOnlyCollection<CustomAttributeTypedArgument>) data.Value)
            {
                if (sb.Length > 0)
                {
                    sb.Append(';');
                }
                sb.Append(item.Value);
            }
            return sb.ToString();
        }

        private static IEnumerable<INetSource> CreateCommands(Assembly assembly)
        {
            int count = 0;

            foreach (var type in assembly.GetTypes())
            {
                if (typeof(INetSource).IsAssignableFrom(type))
                {
                    var result = Activator.CreateInstance(type) as INetSource;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }

        
    }
}
