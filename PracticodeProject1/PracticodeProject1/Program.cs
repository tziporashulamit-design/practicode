using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;

// ===== Command & Options =====
var bundleCommand = new Command("bundle", "Bundles files");
var bundleOption = new Option<FileInfo>(
    "--output",
    "Path to output file"
)
{ IsRequired = true };

var languageOption = new Option<string[]>(
    "--language",
    "Programming languages to include"
)
{ IsRequired = true };
languageOption.AddAlias("-l");

var sortOption = new Option<string>(
    "--sort",
    () => "name",
    "Sort files by name or type"
);
sortOption.AddAlias("-s");
sortOption.FromAmong("name", "type");

var removeEmptyLinesOption = new Option<bool>(
    "--remove-empty-lines",
    "delete empty lines"
);
removeEmptyLinesOption.AddAlias("-r");
var noteOption = new Option<bool>(
    "--note",
    "write note about file"
);
noteOption.AddAlias("-n");

var authorOption = new Option<string>(
    "--author",
    "write author"
);
authorOption.AddAlias("-a");

var createRspCommand = new Command("create-rsp","Interactively creates a response file for bundle command");
createRspCommand.SetHandler(() =>
{
    Console.WriteLine("Creating bundle response file");

    Console.Write("Response file name (e.g. bundle.rsp): ");
    var rspFileName = Console.ReadLine();

    using var writer = new StreamWriter(rspFileName);

    writer.WriteLine("bundle");

    Console.Write("Output file path: ");
    var output = Console.ReadLine();
    writer.WriteLine($"--output {output}");

    Console.Write("Languages (cs, js, py, all): ");
    var languages = Console.ReadLine();
    writer.WriteLine($"--language {languages}");

    Console.Write("Sort by (name/type): ");
    var sort = Console.ReadLine();
    writer.WriteLine($"--sort {sort}");

    Console.Write("Remove empty lines? (y/n): ");
    if (Console.ReadLine()?.ToLower() == "y")
        writer.WriteLine("--remove-empty-lines");

    Console.Write("Add source note? (y/n): ");
    if (Console.ReadLine()?.ToLower() == "y")
        writer.WriteLine("--note");

    Console.Write("Author (optional): ");
    var author = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(author))
        writer.WriteLine($"--author {author}");

    Console.WriteLine($"Response file '{rspFileName}' created successfully");
});



bundleCommand.AddOption(bundleOption);
bundleCommand.AddOption(languageOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(removeEmptyLinesOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(authorOption);

// ===== Language map =====

var languageExtensions = new Dictionary<string, string[]>
{
    ["cs"] = new[] { ".cs" },
    ["js"] = new[] { ".js" },
    ["ts"] = new[] { ".ts" },
    ["py"] = new[] { ".py" },
    ["java"] = new[] { ".java" }
};

// ===== Helper =====

static string[] GetExtensions(
    string[] languages,
    Dictionary<string, string[]> languageExtensions)
{
    if (languages.Contains("all"))
        return new[] { "*" };

    return languages
        .Where(l => languageExtensions.ContainsKey(l))
        .SelectMany(l => languageExtensions[l])
        .ToArray();
}

// ===== Handler =====

bundleCommand.SetHandler(

    (FileInfo output, string[] language, string sort, bool removeEmptyLines, bool note, string author) =>
    {
 
        if (output == null)
    {
  Console.WriteLine("Error: --output is required");
  return;
      }
        if (language == null || language.Length == 0)
        {
            Console.WriteLine("Error: --language is required");
     return;
    }

    Console.WriteLine($"removeEmptyLines = {removeEmptyLines}");

        var baseDir = Directory.GetCurrentDirectory();
        var excludedFolders = new[] { "bin", "obj", "debug" };
        var outputFullPath = Path.GetFullPath(output.FullName);

        var extensions = GetExtensions(language, languageExtensions);

  var files = Directory

            .EnumerateFiles(baseDir, "*.*", SearchOption.AllDirectories)
            .Where(f =>
          !excludedFolders.Any(ex =>
         f.Contains(
          Path.DirectorySeparatorChar + ex + Path.DirectorySeparatorChar,
        StringComparison.OrdinalIgnoreCase)
        )
            );
        files = files.Where(f =>
    !f.EndsWith(".rsp", StringComparison.OrdinalIgnoreCase)
);
        files = files.Where(f => Path.GetFullPath(f) != outputFullPath);

    if (!extensions.Contains("*"))
      {
            files = files.Where(f =>
        extensions.Contains(
              Path.GetExtension(f),
              StringComparer.OrdinalIgnoreCase
  )
      );
    }

        files = sort == "type"
    ? files.OrderBy(f => Path.GetExtension(f))
  : files.OrderBy(f => Path.GetFileName(f));

        var fileList = files.ToList();

   using var writer = new StreamWriter(output.FullName, false);
        if(author!=null)
    writer.WriteLine($"// ===== {author} =====");

  foreach (var file in fileList)
        {
            writer.WriteLine($"// ===== {Path.GetFileName(file)} =====");

            var lines = File.ReadAllLines(file);

            if (removeEmptyLines)
            {
                lines = lines
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToArray();
            }
            if (note)
            {
               var relativePath= Path.GetRelativePath(baseDir, file);
                writer.WriteLine($"// Source: {relativePath}");
            }
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }

            writer.WriteLine();
        }


        Console.WriteLine("Bundle created successfully:");
        Console.WriteLine(output.FullName);
    },
    bundleOption,
    languageOption,
    sortOption,
    removeEmptyLinesOption,
    noteOption,
    authorOption);

// ===== Root =====

var rootCommand = new RootCommand("Root command for file bundler CLI");
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(createRspCommand);

var parser = new CommandLineBuilder(rootCommand)
    .UseDefaults()
 .Build();

await parser.InvokeAsync(args);
