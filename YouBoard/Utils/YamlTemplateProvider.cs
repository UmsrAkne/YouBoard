using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YouBoard.Utils
{
    public static class YamlTemplateProvider
    {
        private readonly static string TemplateDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "local_data", "yaml_templates/");

        public static string GetTemplate(string name)
        {
            var file = Path.Combine(TemplateDir, $"{name}.yaml");

            // 初回起動 or 消された場合のフォールバック
            if (!File.Exists(file))
            {
                Directory.CreateDirectory(TemplateDir);
                var defaultText = LoadEmbeddedDefault(name);
                File.WriteAllText(file, defaultText);
            }

            return File.ReadAllText(file, Encoding.UTF8);
        }

        public static IEnumerable<string> ListTemplates()
        {
            if (!Directory.Exists(TemplateDir))
            {
                return Enumerable.Empty<string>();
            }

            return Directory.GetFiles(TemplateDir, "*.yaml")
                .Select(Path.GetFileNameWithoutExtension);
        }

        private static string LoadEmbeddedDefault(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var path = $"YouBoard.Resources.Templates.{name}.yaml";

            using var s = asm.GetManifestResourceStream(path)
                          ?? throw new FileNotFoundException($"Embedded default template '{name}' not found.");

            using var r = new StreamReader(s);
            return r.ReadToEnd();
        }
    }
}