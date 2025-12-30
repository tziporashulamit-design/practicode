using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace practicodeProject2;
public class HtmlSerializer
{
    
    public async Task<string> Load(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }
    }

    public List<string> Parse(string html)
    {
    
        string cleanedHtml = Regex.Replace(html, @"\s+", " ");

     
        var tokens = Regex.Matches(cleanedHtml, @"(<[^>]*>|[^<]+)")
                          .Cast<Match>()
                          .Select(m => m.Value.Trim())
                          .Where(v => !string.IsNullOrWhiteSpace(v))
                          .ToList();

        return tokens;
    }
    public HtmlElement BuildTree(List<string> tokens)
        {
            HtmlElement root = new HtmlElement { Name = "root" };
            HtmlElement currentElement = root;

            foreach (var token in tokens)
            {                
                string firstWord = token.Trim('<', '>').Split(' ')[0];
                if (firstWord == "/html")
                {break;}
                else if (firstWord.StartsWith("/"))
                {
                    if (currentElement.Parent != null)
                    {
                        currentElement = currentElement.Parent;
                    }
                }
                else if (HtmlHelper.Instance.AllTags.Contains(firstWord))
                {
                    HtmlElement newElement = new HtmlElement
                    {
                        Name = firstWord,
                        Parent = currentElement
                    };

                   
                    var attrMatches = Regex.Matches(token, "([^\\s]*?)=\"(.*?)\"");
                    foreach (Match match in attrMatches)
                    {
                        string name = match.Groups[1].Value;
                        string value = match.Groups[2].Value;

                        if (name.ToLower() == "id")
                            newElement.Id = value;
                        else if (name.ToLower() == "class")
                            newElement.Classes.AddRange(value.Split(' '));
                        else
                            newElement.Attributes[name] = value;
                    }

                    currentElement.Children.Add(newElement);
                    bool isSelfClosing = token.EndsWith("/>") || HtmlHelper.Instance.SelfClosingTags.Contains(firstWord);

                    if (!isSelfClosing)
                    {
                        currentElement = newElement; 
                    }
                }
                else
                { 
                    currentElement.InnerHtml = token;
                }
            }

            return root.Children.FirstOrDefault();
        }
    }
