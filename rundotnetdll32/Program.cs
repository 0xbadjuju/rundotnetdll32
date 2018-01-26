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
                    Type type = assembly.GetType(assembly.FullName.Split(',')[0] + "." + className);
                    MethodInfo methodInfo = type.GetMethod(method);
                    String returnValue = (String)methodInfo.Invoke(null, arguments);

                    Console.WriteLine(returnValue);
                }
                else
                {
                    Console.WriteLine("rundotnetdll32.exe assembly.dll,class,method arguments");
                }
            }
        }
    }
}
