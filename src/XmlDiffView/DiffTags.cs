namespace Microsoft.XmlDiffPatch.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class DiffTags
    {
        public static string GetXmlInsertString(string text)
        {
            var tag = "<INS STYLE=\"background:#E6FFE6;\">{0}</INS>";
            return string.Format(tag, text);
        }

        public static string GetXmlDeleteString(string text)
        {
            var tag = "<DEL STYLE=\"background:#FFE6E6;\">{0}</DEL>";
            return string.Format(tag, text);
        }

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
