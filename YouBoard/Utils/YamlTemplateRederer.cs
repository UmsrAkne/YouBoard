using System;
using System.Collections.Generic;
using Scriban;
using Scriban.Runtime;

namespace YouBoard.Utils;

public static class YamlTemplateRenderer
{
    public static string Render(string templateText, Dictionary<string, object> data)
    {
        // Scriban テンプレートをパース
        var template = Template.Parse(templateText);

        if (template.HasErrors)
        {
            // 必要に応じてログや例外
            throw new InvalidOperationException(
                "Template parse error: " + string.Join("\n", template.Messages));
        }

        // Scriban が Dictionary を自然に扱えるようにオブジェクトをそのまま渡す
        var ctx = new TemplateContext();
        var scriptObj = new ScriptObject();

        foreach (var kv in data)
        {
            scriptObj.Add(kv.Key, kv.Value);
        }

        ctx.PushGlobal(scriptObj);

        return template.Render(ctx);
    }
}