using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YouBoard.Utils
{
    public static class YamlTemplateProvider
    {
        // ReSharper disable once ArrangeModifiersOrder
        private static readonly string TemplateDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "local_data", "yaml_templates/");

        public static string GetTemplate(string name)
        {
            var file = Path.Combine(TemplateDir, $"{name}.yaml");

            // 初回起動 or 消された場合のフォールバック
            if (!File.Exists(file))
            {
                Directory.CreateDirectory(TemplateDir);
                var defaultText = GetDefaultTemplate();
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

        private static string GetDefaultTemplate()
        {
            return "id: {{ Id }}\r\n"
                    + "title: {{ Title }}\r\n"
                    + "state: {{ State }}\r\n"
                    + "entry: {{ EntryNo }}\r\n"
                    + "description: |\r\n"
                    + "  {{ Description }}\r\n"
                    + "comments:\r\n"
                    + "{{ for c in Comments }}\r\n"
                    + "  - text: | {{ c.Text }} ({{ c.Created }})\r\n"
                    + "{{ end }}\r\n"
                    + "{{ End }}\r\n";
        }
    }
}