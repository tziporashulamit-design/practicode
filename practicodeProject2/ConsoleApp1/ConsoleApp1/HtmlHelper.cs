using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace practicodeProject2;
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;

        public string[] AllTags { get; set; }
        public string[] SelfClosingTags { get; set; }

        private HtmlHelper()
        {
        var allTagsContent = File.ReadAllText("HtmlTags.json");
        var selfClosingTagsContent = File.ReadAllText("HtmlVoidTags.json");
        AllTags = JsonSerializer.Deserialize<string[]>(allTagsContent);
            SelfClosingTags = JsonSerializer.Deserialize<string[]>(selfClosingTagsContent);
        }
    }

