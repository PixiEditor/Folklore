
using System.Diagnostics;
using System.Text;
using Folklore;
using Folklore.Intermediate;
using Folklore.LLVMCompiler;
using Folklore.NetILTranspiler;

string input = string.Empty;
if (args.Length == 0)
{
    StringBuilder inputBuilder = new StringBuilder();
    Console.WriteLine("Write your Folklore code below. Type 'exit' on a new line to finish input:");

    while (true)
    {
        string? line = Console.ReadLine();
        if (line == null || line.Trim().ToLower() == "exit")
            break;

        inputBuilder.AppendLine(line);
    }

    input = inputBuilder.ToString();
}
else if (args.Length == 1 && Path.GetExtension(args[0]) == ".pxs" && File.Exists(args[0]))
{
    input = File.ReadAllText(args[0]);
}

var parsed = Parser.Parse(input, out string[]? errors);

if (parsed == null)
{
    Console.WriteLine("Parsing failed with errors:");
    if (errors != null)
    {
        foreach (var error in errors)
        {
            Console.WriteLine($"- {error}");
        }
    }
    return;
}

ISyntaxTreeCompiler compiler = new DotNetILCompiler();
Console.WriteLine($"Parsing succeeded. Compiling with {compiler.Name}...");
Stopwatch sw = Stopwatch.StartNew();
var main = compiler.DynamicCompile(parsed);
sw.Stop();
Console.WriteLine("Compilation succeeded in {0} ms.", sw.ElapsedMilliseconds);
Console.WriteLine("Executing compiled Folklore...");

try
{
    main();
}
catch (Exception ex)
{
    Console.WriteLine($"Execution failed with exception: {ex.Message}");
}