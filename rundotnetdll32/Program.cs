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
            Assembly assembly = null;
            if (args.Length >= 1)
            {
                String file = Path.GetFullPath(args[0].Split(',')[0]);
                if (!File.Exists(file))
                {
                    Console.WriteLine("[-] File Not Found");
                    return;
                }
                assembly = Assembly.LoadFile(Path.GetFullPath(args[0].Split(',')[0]));
            }
            else
            {
                Console.WriteLine("rundotnetdll32.exe assembly.dll,class,method arguments");
                Console.WriteLine("rundotnetdll32.exe list <namespaces|classes|methods> <namespace> <class>");
                return;
            }

            if (args.Length <= 2)
            {
                String[] dllArgs = args[0].Split(',');
                if (dllArgs.Length >= 3)
                {
                    String nameSpace = dllArgs[1];
                    String className = dllArgs[2];
                    String method = dllArgs[3];
                    String[] arguments = new String[0];
                    if (2 == args.Length)
                    {
                        arguments = args.Skip(1).Take(args.Length - 1).ToArray()[0].Split(',');
                    }

                    Console.WriteLine("----------");
                    Console.WriteLine("Namespace: {0}\nClass: {1}\nMethod: {2}\nArguments: {3}", 
                        nameSpace, className, method, String.Join(" ",arguments));
                    Console.WriteLine("----------");

                    String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray(); 
                    foreach (String space in namespaceNames)
                    {
                        if (space != nameSpace)
                        {
                            continue;
                        }
                        try
                        {
                            Type type = assembly.GetType(space + "." + className);
                            MethodInfo methodInfo = type.GetMethod(method);
                            Console.WriteLine((String)methodInfo.Invoke(null, arguments));
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine(error);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("rundotnetdll32.exe assembly.dll,class,method arguments");
                }
            }
            else if (args.Length >= 3 && args[1] == "list")
            {
                String lookup = args[2].ToLower();

                String specificNamespace = null;
                if (args.Length >= 4)
                {
                    specificNamespace = args[3];
                }

                String specificClassname = null;
                if (args.Length >= 5)
                {
                    specificClassname = args[4];
                }

                if (lookup == "namespaces")
                {
                    Namepaces(assembly);
                }
                else if ("classes" == lookup && null == specificNamespace)
                {
                    Classes(assembly);
                }
                else if ("classes" == lookup && null != specificNamespace)
                {
                    Classes(assembly, specificNamespace);
                }
                else if ("methods" == lookup && null == specificNamespace)
                {
                    Methods(assembly);
                }
                else if ("methods" == lookup && null != specificNamespace && null == specificClassname)
                {
                    Methods(assembly, specificNamespace);
                }
                else if ("methods" == lookup && null != specificNamespace && null != specificClassname)
                {
                    Methods(assembly, specificNamespace, specificClassname);
                }
                else
                {
                    Console.WriteLine("rundotnetdll32.exe list <namespaces|classes|methods> <namespace> <class>");
                }
            }
        }

        #region listProperties
        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void Namepaces(Assembly assembly)
        {
            String[] namespaceNames = assembly.GetTypes().Select(n => n.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                Console.WriteLine(space);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void Classes(Assembly assembly)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                Console.WriteLine(space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == space).Distinct().ToArray();
                    foreach (Type t in types)
                    {
                        Console.WriteLine("\t{0}", t.Name);
                    }
                }
                catch
                {
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void Classes(Assembly assembly, String specificNamespace)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                Console.WriteLine(space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == specificNamespace).Distinct().ToArray();
                    foreach (Type t in types)
                    {
                        Console.WriteLine("\t{0}", t.Name);
                    }
                }
                catch
                {
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void Methods(Assembly assembly)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                Console.WriteLine(space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == space).Distinct().ToArray();
                    foreach (Type className in types)
                    {
                        Console.WriteLine("\t{0}", className.Name);
                        foreach (MethodInfo method in className.GetMethods())
                        {
                            Console.WriteLine("\t\t{0}", method.Name);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void Methods(Assembly assembly, String specificNamespace)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                Console.WriteLine(space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == specificNamespace).Distinct().ToArray();
                    foreach (Type className in types)
                    {
                        Console.WriteLine("\t{0}", className.Name);
                        foreach (MethodInfo method in className.GetMethods())
                        {
                            Console.WriteLine("\t\t{0}", method.Name);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void Methods(Assembly assembly, String specificNamespace, String specificClassName)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                Console.WriteLine(space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == specificNamespace).Distinct().ToArray();
                    foreach (Type className in types)
                    {
                        if (specificClassName == className.Name)
                        {
                            Console.WriteLine("\t{0}", className.Name);
                            foreach (MethodInfo method in className.GetMethods())
                            {
                                Console.WriteLine("\t\t{0}", method.Name);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}
