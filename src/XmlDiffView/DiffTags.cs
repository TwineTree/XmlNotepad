namespace Microsoft.XmlDiffPatch.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class DiffTags
    {
        public const string START_INSERT_TAG = "<INS STYLE=\"background:#E6FFE6;\">";
        public const string END_INSERT_TAG = "</INS>";

        public const string START_DELETE_TAG = "<DEL STYLE=\"background:#FFE6E6;\">";
        public const string END_DELETE_TAG = "</DEL>";

        public const string START_INSERT_SVG_TAG = "<svg:tspan fill=\"red\" style=\"text-decoration:line-through;\">";
        public const string END_INSERT_SVG_TAG = "</svg:tspan>";

        public static string GetSvgInsertString(string text)
        {
            var tag = "<svg:tspan fill=\"green\">{0}</svg:tspan>";
            return string.Format(tag, text);
        }

        public static string GetSvgDeleteString(string text)
        {
            var tag = "<svg:tspan fill=\"red\" style=\"text-decoration:line-through;\">{0}</svg:tspan>";
            return string.Format(tag, text);
        }
    }
}
