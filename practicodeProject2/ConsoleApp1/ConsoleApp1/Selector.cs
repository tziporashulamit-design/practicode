using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace practicodeProject2;
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new();

        public Selector Parent { get; set; }
        public Selector Child { get; set; }
    public static Selector Parse(string query)
    {        var parts = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Selector root = null;
        Selector current = null;

        foreach (var part in parts)
        {            Selector newSelector = new Selector();
            var matches = Regex.Matches(part, @"(^[\w-]+)|(#[\w-]+)|(\.[\w-]+)");

            foreach (Match match in matches)
            {
                string value = match.Value;

                if (value.StartsWith("#"))
                {
                    newSelector.Id = value.Substring(1); 
                }
                else if (value.StartsWith("."))
                {
                    newSelector.Classes.Add(value.Substring(1)); 
                }
                else
                {
                    if (HtmlHelper.Instance.AllTags.Contains(value.ToLower()))
                    {
                        newSelector.TagName = value.ToLower();
                    }
                }
            }

            if (root == null)
            {
                root = newSelector;
            }
            else
            {
                current.Child = newSelector;
                newSelector.Parent = current;
            }
            current = newSelector;
        }

        return root;
    }
}


