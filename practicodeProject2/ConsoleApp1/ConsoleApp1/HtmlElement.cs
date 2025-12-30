using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practicodeProject2;
    public class HtmlElement
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new();
        public Dictionary<string, string> Attributes { get; set; } = new();
        public string InnerHtml { get; set; }

        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new();
        public HtmlElement()
        {
            Attributes = new Dictionary<string, string>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }
    public IEnumerable<HtmlElement> Descendants()
    {
        Queue<HtmlElement> queue = new Queue<HtmlElement>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            HtmlElement current = queue.Dequeue();
            yield return current;

            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }
    }
    public IEnumerable<HtmlElement> Ancestors()
    {
        HtmlElement current = this.Parent;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }
}

