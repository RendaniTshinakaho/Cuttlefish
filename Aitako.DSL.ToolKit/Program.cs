using System;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;

namespace Aitako.DSL.ToolKit
{
    internal class Program
    {
        public static void RunTransformations(string userCode)
        {
            using (var codeProvider = new CSharpCodeProvider())
            {
                string code =
                    @"using Aitako.DSL;using System; using Aitako.DSL.Components;using System.Collections.Generic;  public class Runner { public void Execute() { " +
                    userCode + " Service.Execute();}}";

                CompilerResults compilerResult =
                    codeProvider.CompileAssemblyFromSource(
                        new CompilerParameters(new[] {"Aitako.DSL.dll"}) {GenerateInMemory = true}, code);

                if (compilerResult.Errors.Count > 0)
                {
                    foreach (object error in compilerResult.Errors)
                    {
                        Console.WriteLine(error.ToString());
                    }
                    return;
                }

                Type compiledType = compilerResult.CompiledAssembly.GetType("Runner");
                object instance = Activator.CreateInstance(compiledType);
                object output = compiledType.GetMethod("Execute").Invoke(instance, new object[] {});

                Console.WriteLine(output);
            }
        }

        public static void Describe(string userCode)
        {
            using (var codeProvider = new CSharpCodeProvider())
            {
                string code =
                    @"using Aitako.DSL;using Aitako.DSL.Components;using System; using System.Collections.Generic;  public class Runner { public void Execute() {     " +
                    userCode + " foreach (var aggregate in Service.All){Console.WriteLine(aggregate.ToString());}}}";

                CompilerResults compilerResult =
                    codeProvider.CompileAssemblyFromSource(
                        new CompilerParameters(new[] {"Aitako.DSL.dll"}) {GenerateInMemory = true}, code);

                if (compilerResult.Errors.Count > 0)
                {
                    foreach (object error in compilerResult.Errors)
                    {
                        Console.WriteLine(error.ToString());
                    }

                    return;
                }

                Type compiledType = compilerResult.CompiledAssembly.GetType("Runner");
                object instance = Activator.CreateInstance(compiledType);
                object output = compiledType.GetMethod("Execute").Invoke(instance, new object[] {});

                Console.WriteLine(output);
            }
        }

        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: executable make <definition file> or executable describe <definition file>");
                Environment.Exit(0);
            }

            string command = args[0];
            string file = args[1];

            using (StreamReader reader = File.OpenText(file))
            {
                string definition = reader.ReadToEnd();

                switch (command)
                {
                    case "describe":
                        Describe(definition);
                        break;
                    case "make":
                        RunTransformations(definition);
                        break;
                }
            }
        }
    }
}