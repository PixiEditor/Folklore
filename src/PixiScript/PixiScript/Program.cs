
using System.Diagnostics;
using System.Text;
using PixiScript;
using PixiScript.Intermediate;
using PixiScript.LLVMCompiler;
using PixiScript.NetILTranspiler;

StringBuilder inputBuilder = new StringBuilder();
Console.WriteLine("Write your PixiScript code below. Type 'exit' on a new line to finish input:");

while (true)
{
    string? line = Console.ReadLine();
    if (line == null || line.Trim().ToLower() == "exit")
        break;

    inputBuilder.AppendLine(line);
}

string input = inputBuilder.ToString();

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

ISyntaxTreeCompiler compiler = new LlvmCompiler();
Console.WriteLine($"Parsing succeeded. Compiling with {compiler.Name}...");
Stopwatch sw = Stopwatch.StartNew();
var main = compiler.DynamicCompile(parsed);
sw.Stop();
Console.WriteLine("Compilation succeeded in {0} ms.", sw.ElapsedMilliseconds);
Console.WriteLine("Executing compiled PixiScript...");

try
{
    main();
}
catch (Exception ex)
{
    Console.WriteLine($"Execution failed with exception: {ex.Message}");
}