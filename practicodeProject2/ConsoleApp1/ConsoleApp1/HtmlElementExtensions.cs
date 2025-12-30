using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practicodeProject2
{
    public static class HtmlElementExtensions
    {
        public static IEnumerable<HtmlElement> FindElements(this HtmlElement element, Selector selector)
        {
            HashSet<HtmlElement> results = new HashSet<HtmlElement>();

            FindElementsRecursive(element, selector, results);

            return results;}
        private static void FindElementsRecursive(HtmlElement currentElement, Selector selector, HashSet<HtmlElement> results)
        {
            var descendants = currentElement.Descendants();
            var matches = descendants.Where(e => IsMatch(e, selector));
            if (selector.Child == null)
            {
                foreach (var match in matches)
                {results.Add(match);}
            }
            else
            {
                foreach (var match in matches)
                {
                    FindElementsRecursive(match, selector.Child, results);
                }
            }
        }
        private static bool IsMatch(HtmlElement element, Selector selector)
        {
            if (selector.TagName != null && element.Name != selector.TagName)
                return false;

            if (selector.Id != null && element.Id != selector.Id)
                return false;

            if (selector.Classes.Any() && !selector.Classes.All(c => element.Classes.Contains(c)))
                return false;

            return true;
        }
    }
}
