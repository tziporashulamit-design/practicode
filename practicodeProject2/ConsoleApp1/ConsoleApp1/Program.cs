using System;
using System.Linq;
using practicodeProject2;

var url = "https://learn.microsoft.com/en-us/dotnet/csharp/";
var serializer = new HtmlSerializer();
var html = await serializer.Load(url);


var tokens = serializer.Parse(html);
var root = serializer.BuildTree(tokens);

Console.WriteLine($"[V]  the tree build the root is: <{root.Name}>");

var allElements = root.Descendants().ToList();
Console.WriteLine($"[V] found {allElements.Count} elements on the tree.");

var query = "a.inner-focus";
var selector = Selector.Parse(query);
var results = root.FindElements(selector);

Console.WriteLine($"[V]result'{query}': {results.Count()} elements.");

if (results.Any())
{
    var first = results.First();
    Console.WriteLine($"example: <{first.Name}> ID: {first.Id} Classes: {string.Join(".", first.Classes)}");
}