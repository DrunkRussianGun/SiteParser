using System;
using System.Collections.Generic;

namespace SiteParser.Implementation.Scan
{
    internal class ParsedHtml
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public List<Uri> Links { get; set; }
    }
}
