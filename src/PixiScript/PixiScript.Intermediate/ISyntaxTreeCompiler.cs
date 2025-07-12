namespace PixiScript.Intermediate;

public interface ISyntaxTreeCompiler
{
    public string Name { get; }
    /// <summary>
    /// Compiles the provided syntax tree into the executable method.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree to compile.</param>
    /// <returns>An action that represents the compiled method.</returns>
    Action DynamicCompile(SyntaxTree syntaxTree);

}