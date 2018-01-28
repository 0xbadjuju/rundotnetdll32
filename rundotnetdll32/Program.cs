using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace rundotnetdll32
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                String[] dllArgs = args[0].Split(',');
                if (dllArgs.Length == 3)
                {
                    String fileName = dllArgs[0];
                    String className = dllArgs[1];
                    String method = dllArgs[2];
                    String[] arguments = args.Skip(1).Take(args.Length - 1).ToArray();

                    Assembly assembly = Assembly.LoadFile(Path.GetFullPath(fileName));
                    String[] namespaceNames = assembly.GetTypes().Select(n => n.Namespace).Distinct().ToArray();
                    foreach (String space in namespaceNames)
                    {
                        try
                        {
                            Type type = assembly.GetType(space + "." + className);
                            MethodInfo methodInfo = type.GetMethod(method);
                            Console.WriteLine((String)methodInfo.Invoke(null, arguments));
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    Console.WriteLine("rundotnetdll32.exe assembly.dll,class,method arguments");
                }
            }
        }
    }
}
