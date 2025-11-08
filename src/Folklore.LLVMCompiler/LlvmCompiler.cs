using System.Globalization;
using Folklore.Intermediate;
using Folklore.Logical;
using Folklore.Syntax;
using Folklore.Types;
using Folklore.Types.Primitive;
using LLVMSharp.Interop;

namespace Folklore.LLVMCompiler;

public class LlvmCompiler : ISyntaxTreeCompiler
{
    private LLVMValueRef printfFunc;
    private LLVMTypeRef printfType;

    public string Name { get; } = "LLVM";

    public unsafe Action DynamicCompile(SyntaxTree syntaxTree)
    {
        LLVMModuleRef module = LLVMModuleRef.CreateWithName("PixiScriptNode");
        LLVMBuilderRef builder = LLVMBuilderRef.Create(LLVMContextRef.Global);

        if (!module.TryCreateExecutionEngine(out LLVMExecutionEngineRef ee, out var err))
        {
            Console.WriteLine("Error " + err);
            return null;
        }

        LLVMOpaqueType*[] printfArgs =
        [
            LLVM.PointerType(LLVM.Int8Type(), 0) // char*
        ];

        fixed (LLVMOpaqueType** argsPtr = printfArgs)
        {
            // Create function type with varargs = true
            printfType = LLVM.FunctionType(LLVM.Int32Type(), argsPtr, (uint)printfArgs.Length, 1);
        }

        printfFunc = module.AddFunction("printf", printfType);

        LLVMTypeRef returnType = LLVMTypeRef.Int32;
        LLVMTypeRef[] paramTypes = [];
        LLVMTypeRef functionType = LLVMTypeRef.CreateFunction(returnType, paramTypes, false);
        LLVMValueRef mainFunction = module.AddFunction("main", functionType);
        LLVMBasicBlockRef entry = mainFunction.AppendBasicBlock("entry");
        builder.PositionAtEnd(entry);

        GenerateMainMethod(syntaxTree, builder);

        return () => { ee.RunFunctionAsMain(mainFunction, 0, ReadOnlySpan<string>.Empty, ReadOnlySpan<string>.Empty); };
    }

    private unsafe void GenerateMainMethod(SyntaxTree syntaxTree, LLVMBuilderRef builder)
    {
        Dictionary<string, LLVMValueRef> variables = new();
        syntaxTree.Traverse((previous, n) =>
        {
            if (n is VariableDeclaration declaration)
            {
                var mappedType = MapVariableTypeToLocalType(declaration.VariableType);
                var alloca = GenLocal(builder, mappedType, declaration.VariableName);
                variables[declaration.VariableName] = alloca;
            }

            if (n is Assignment assignment)
            {
                if (assignment.AssignedConstantLiteral != null)
                {
                    if (!variables.TryGetValue(assignment.AssignTo.Name, out var varPtr))
                        throw new Exception($"Variable '{assignment.AssignTo.Name}' not found");

                    AssignTo(builder, varPtr, assignment.AssignedConstantLiteral);
                }
            }

            if (n is Call call)
            {
                if (call.FunctionName == "log")
                {
                    string message = call.Arguments.Count > 0 ? call.Arguments[0] : string.Empty;
                    LLVMValueRef local;
                    if (variables.TryGetValue(message, out var varPtr))
                    {
                        local = varPtr;
                    }
                    else
                    {
                        local = builder.BuildGlobalStringPtr(message);
                    }

                    ConsoleLog(builder, local);
                }
            }

            if (n is MathExpression mathExpr)
            {
                throw new NotImplementedException("Math expressions are not implemented in LLVM compiler yet.");
            }
        });

        builder.BuildRet(LLVM.ConstInt(LLVM.Int32Type(), 0, 0));
    }

    private unsafe void ConsoleLog(LLVMBuilderRef builder, LLVMValueRef local)
    {
        var loaded = builder.BuildLoad2(local.TypeOf, local);
        var formatStr = builder.BuildGlobalStringPtr("%f\n");

        LLVMValueRef[] args = { formatStr, loaded };
        builder.BuildCall2(printfType, printfFunc, args, "call");
    }


    private unsafe void AssignTo(LLVMBuilderRef builder, LLVMValueRef varPtr, Literal literal)
    {
        if (literal is Literal<int> intType)
        {
            AssignInt(builder, varPtr, intType.Value);
            return;
        }

        if (literal is Literal<double> doubleType)
        {
            AssignDouble(builder, varPtr, doubleType.Value);
            return;
        }
    }

    private static unsafe void AssignInt(LLVMBuilderRef builder, LLVMValueRef varPtr, int value)
    {
        const uint NumBits = 32;
        var valRef = LLVM.ConstReal(LLVM.IntType(NumBits), value);
        builder.BuildStore(valRef, varPtr);
    }

    private static unsafe void AssignDouble(LLVMBuilderRef builder, LLVMValueRef varPtr, double value)
    {
        var valRef = LLVM.ConstReal(LLVM.DoubleType(), value);
        builder.BuildStore(valRef, varPtr);
    }

    private unsafe LLVMValueRef GenLocal(LLVMBuilderRef builder, LLVMOpaqueType* mappedType,
        string declarationVariableType)
    {
        return builder.BuildAlloca(mappedType, declarationVariableType);
    }

    private unsafe LLVMOpaqueType* MapVariableTypeToLocalType(FolkloreType varType)
    {
        return varType switch
        {
            NumberType => LLVM.DoubleType(),
            _ => throw new NotSupportedException($"Variable type '{varType}' is not supported.")
        };
    }
}