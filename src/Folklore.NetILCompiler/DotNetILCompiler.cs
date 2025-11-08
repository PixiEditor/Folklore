using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Folklore.Intermediate;
using Folklore.Syntax;
using Folklore.Logical;
using Folklore.Types;
using Folklore.Types.Primitive;

namespace Folklore.NetILTranspiler;

public class DotNetILCompiler : ISyntaxTreeCompiler
{
    public string Name { get; } = ".NET IL Compiler";

    public Action DynamicCompile(SyntaxTree syntaxTree)
    {
        // use ILGenerator
        AssemblyName assemblyName = new AssemblyName("FolkloreAssembly");
        AssemblyBuilder assemblyBuilder =
            AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var t = moduleBuilder.DefineType("FolkloreProgram", TypeAttributes.Public | TypeAttributes.Class);
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
        syntaxTree.Traverse((previous, n) =>
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
                if (call.FunctionName == "log")
                {
                    string message = call.Arguments.Count > 0 ? call.Arguments[0] : string.Empty;
                    bool isVariable = locals.Any(ld => ld.VariableName == message);
                    LocalBuilder local = isVariable
                        ? localBuilders[locals.FindIndex(ld => ld.VariableName == message)]
                        : generator.DeclareLocal(typeof(string));

                    ConsoleLog(generator, local);
                }
            }

            if (n is MathExpression expression)
            {
                PushOperand(generator, expression.LeftOperand, locals, localBuilders);
                PushOperand(generator, expression.RightOperand, locals, localBuilders);
                EmitMathOperation(generator, expression.Operator.Text);
                if (previous is Assignment assignTo)
                {
                    int localIndex = locals.FindIndex(ld => ld.VariableName == assignTo.AssignTo.Name);
                    PopToVariable(generator, localIndex);
                }
            }
        });

        generator.Emit(OpCodes.Ret);
    }
    
    private void EmitMathOperation(ILGenerator generator, string operatorText)
    {
        switch (operatorText)
        {
            case "+":
                generator.Emit(OpCodes.Add);
                break;
            case "-":
                generator.Emit(OpCodes.Sub);
                break;
            case "*":
                generator.Emit(OpCodes.Mul);
                break;
            case "/":
                generator.Emit(OpCodes.Div);
                break;
            default:
                throw new NotSupportedException($"Operator '{operatorText}' is not supported.");
        }
    }

    private void PushOperand(ILGenerator generator, Operand? leftOperand, List<VariableDeclaration> locals, List<LocalBuilder> localBuilders)
    {
        if (leftOperand!.IsLiteral)
        {
            PushConstant(generator, leftOperand.LiteralValue!);
        }
        else
        {
            int localIndex = locals.FindIndex(ld => ld.VariableName == leftOperand.ReferenceValue.Name);
            generator.Emit(OpCodes.Ldloc, localBuilders[localIndex]);
        }
    }

    private void ConsoleLog(ILGenerator generator, LocalBuilder local)
    {
        generator.EmitWriteLine(local);
    }

    private void PushConstant(ILGenerator sb, Literal constant)
    {
        if (constant is Literal<string> textType)
        {
            sb.Emit(OpCodes.Ldstr, textType.Value);
            return;
        }

        if (constant is Literal<int> intType)
        {
            sb.Emit(OpCodes.Ldc_I4, intType.Value);
            sb.Emit(OpCodes.Conv_R8); // Convert to double
            return;
        }
        if (constant is Literal<double> doubleType)
        {
            sb.Emit(OpCodes.Ldc_R8, doubleType.Value);
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

    private Type MapVariableTypeToLocalType(FolkloreType variableType)
    {
        return variableType switch
        {
            NumberType => typeof(double),
            TextType => typeof(string),
            _ => throw new NotSupportedException($"Variable type '{variableType}' is not supported.")
        };
    }
}