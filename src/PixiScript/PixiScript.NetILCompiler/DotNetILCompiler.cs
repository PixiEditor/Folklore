using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using PixiScript.Intermediate;
using PixiScript.Logical;
using PixiScript.Syntax;

namespace PixiScript.NetILTranspiler;

public class DotNetILCompiler : ISyntaxTreeCompiler
{
    public Action DynamicCompile(SyntaxTree syntaxTree)
    {
        // use ILGenerator
        AssemblyName assemblyName = new AssemblyName("PixiScriptAssembly");
        AssemblyBuilder assemblyBuilder =
            AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var t = moduleBuilder.DefineType("PixiScriptProgram", TypeAttributes.Public | TypeAttributes.Class);
        MethodBuilder methodBuilder = t.DefineMethod(
            "<Main>$",
            MethodAttributes.Public | MethodAttributes.Static,
            typeof(void),
            Type.EmptyTypes);

        ILGenerator ilGenerator = methodBuilder.GetILGenerator();

        GenerateMainMethod(syntaxTree, ilGenerator);

        Type programType = t.CreateType();
        MethodInfo mainMethod = programType.GetMethod("<Main>$", BindingFlags.Public | BindingFlags.Static);
        if (mainMethod == null)
        {
            throw new InvalidOperationException("Main method not found in the generated type.");
        }

        DynamicMethod dynamicMethod = new DynamicMethod("ExecuteMain", null, null, programType);
        ILGenerator dynamicIL = dynamicMethod.GetILGenerator();
        dynamicIL.Emit(OpCodes.Call, mainMethod);
        dynamicIL.Emit(OpCodes.Ret);

        Action executeMain = (Action)dynamicMethod.CreateDelegate(typeof(Action));
        return executeMain;
    }

    private void GenerateMainMethod(SyntaxTree syntaxTree, ILGenerator generator)
    {
        List<VariableDeclaration> locals = new List<VariableDeclaration>();
        List<LocalBuilder> localBuilders = new List<LocalBuilder>();
        syntaxTree.Traverse(n =>
        {
            if (n is VariableDeclaration declaration)
            {
                locals.Add(declaration);
                Type mappedType = MapVariableTypeToLocalType(declaration.VariableType);
                localBuilders.Add(generator.DeclareLocal(mappedType));
            }

            if (n is Assignment assignment)
            {
                if (assignment.AssignedConstantLiteral != null)
                {
                    PushConstant(generator, assignment.AssignedConstantLiteral);
                    PopToVariable(generator, locals.FindIndex(ld => ld.VariableName == assignment.AssignTo.Name));
                }
            }

            if (n is Call call)
            {
                if (call.FunctionName == "print")
                {
                    string message = call.Arguments.Count > 0 ? call.Arguments[0] : string.Empty;
                    bool isVariable = locals.Any(ld => ld.VariableName == message);
                    LocalBuilder local = isVariable
                        ? localBuilders[locals.FindIndex(ld => ld.VariableName == message)]
                        : generator.DeclareLocal(typeof(string));

                    ConsoleLog(generator, local);
                }
            }
        });

        generator.Emit(OpCodes.Ret);
    }

    private void ConsoleLog(ILGenerator generator, LocalBuilder local)
    {
        generator.EmitWriteLine(local);
    }

    private void PushConstant(ILGenerator sb, string constant)
    {
        if (double.TryParse(constant, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
        {
            sb.Emit(OpCodes.Ldc_R8, number);
        }
        else
        {
            throw new NotSupportedException($"Constant '{constant}' is not a valid number.");
        }
    }

    private void PopToVariable(ILGenerator generator, int index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");
        }

        // Assuming the variable is a float64
        generator.Emit(OpCodes.Stloc, index);
    }

    private Type MapVariableTypeToLocalType(string variableType)
    {
        return variableType switch
        {
            "number" => typeof(double),
            _ => throw new NotSupportedException($"Variable type '{variableType}' is not supported.")
        };
    }
}