using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Mono.Options;

namespace rundotnetdll32
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                RunApp(args);
            }
            catch(ReflectionTypeLoadException ex)
            {
                Console.WriteLine("[-] Unhandled ReflectionTypeLoadException occurred");
                Console.WriteLine(ex.Message);
                if(ex.LoaderExceptions != null && ex.LoaderExceptions.Length > 0)
                {
                    foreach(var lex in ex.LoaderExceptions)
                    {
                        Console.WriteLine("[-] Loader Exception");
                        Console.WriteLine(lex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("[-] Unhandled exception occurred");
                Console.WriteLine(ex.Message);
            }
        }

        private static void RunApp(String[] args)
        {
            Boolean list = false;
            String namespaceName = String.Empty;
            String className = String.Empty;
            String methodName = String.Empty;
            Boolean format = false;
            Boolean nobanner = false;
            Boolean interactive = false;
            Boolean type = false;
            String delimiter = ",";
            Boolean help = false;

            OptionSet options = new OptionSet() {
                { "l|list", "List All Namespaces Classes Methods.", v => list = v != null },
                { "n|namespace=", "List All Namespaces Classes Methods.", v => namespaceName = v },
                { "c|class=", "List All Namespaces Classes Methods.", v => className = v },
                { "m|method=", "List All Namespaces Classes Methods.", v => methodName = v },
                { "f|format", "List Info in RunDotNetDll32 executable format.", v => format = v != null },
                { "b|nobanner", "Do not display the banner.\nUseful when redirecting directly to a file.", v => nobanner = v != null },
                { "i|interactive", "Interact with the Assembly in a semi interactive shell", v => interactive = v != null },
                { "t|type", "Assembly Exists in the GAC or Current Directory\n Does not call Assembly.LoadFile", v => type = v != null },
                { "d|delimiter=", "Alternate delimeter to use", v => delimiter = v },
                { "h|help",  "Display this message and exit", v => help = v != null }
            };

            List<String> methodAndArgs;
            try
            {
                methodAndArgs = options.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            String file = Path.GetFullPath(methodAndArgs.First().Split(new String[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).First());
            if (String.IsNullOrEmpty(file) || !File.Exists(file))
            {
                Console.WriteLine("[-] File Not Found");
                return;
            }

            if (interactive)
            {
                Interactive i = new Interactive(file);
                i.Execute();
                return;
            }

            if (!list && methodAndArgs.Count > 0)
            {
                String[] dllArgs = methodAndArgs.First().Split(new String[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                if (dllArgs.Length >= 3)
                {
                    namespaceName = dllArgs[1];
                    className = dllArgs[2];
                    methodName = dllArgs[3];
                    String[] arguments = new String[0];
                    if (2 == methodAndArgs.Count)
                    {
                        arguments = methodAndArgs.Skip(1).Take(methodAndArgs.Count - 1).ToArray().First().Split(new String[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    if (!nobanner)
                    {
                        Console.WriteLine("----------");
                        Console.WriteLine("Namespace: {0}\nClass: {1}\nMethod: {2}\nArguments: {3}",
                            namespaceName, className, methodName, String.Join(" ", arguments));
                        Console.WriteLine("----------");
                    }

                    if (type)
                    {
                        TypeLoad(file, namespaceName, className, methodName, arguments);
                    }
                    else
                    {
                        StdLoad(file, namespaceName, className, methodName, arguments);
                    }
                }
                else
                {
                    Console.WriteLine("One of the following is missing:\n Path, Namespace, Class, Method");
                    Help(options);
                }
            }
            else if (list)
            {
                Assembly assembly = Assembly.LoadFile(file);

                if (!String.IsNullOrEmpty(namespaceName))
                {
                    if (!String.IsNullOrEmpty(className))
                    {
                        if (!String.IsNullOrEmpty(methodName))
                        {
                            ListMethodParameters(assembly, namespaceName, className, methodName);
                            return;
                        }
                        ListClassMethods(assembly, namespaceName, className);
                        return;
                    }
                    ListNamespaceClasses(assembly, namespaceName);
                    return;
                }
                ListAllNamepaces(assembly);
            }
        }

        #region execute
        private static void StdLoad(String path, String nameSpace, String className, String method, String[] arguments)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.LoadFile(Path.GetFullPath(path));
            }
            catch (Exception ex)
            {
                if (ex is FileLoadException)
                {
                    Console.WriteLine("[-] File could not be loaded");
                }
                else if (ex is BadImageFormatException)
                {
                    Console.WriteLine("[-] File uses incorrect framework version");
                }
                else
                {
                    Console.WriteLine("[-] Error occured loading file");
                }
                Console.WriteLine(ex);
                return;
            }

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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void TypeLoad(String path, String nameSpace, String className, String method, String[] arguments)
        {
            AssemblyName assemblyName = null;
            try
            {
                assemblyName = AssemblyName.GetAssemblyName(Path.GetFullPath(path));
            }
            catch (Exception ex)
            {
                if (ex is FileLoadException)
                {
                    Console.WriteLine("[-] File could not be loaded");
                }
                else if (ex is BadImageFormatException)
                {
                    Console.WriteLine("[-] File uses incorrect framework version");
                }
                else
                {
                    Console.WriteLine("[-] Error occured loading file");
                }
                Console.WriteLine(ex.Message);
                return;
            }

            try
            {
                String fullClassName = String.Format("{0}.{1}", nameSpace, className);
                Console.WriteLine(String.Format("{0}, {1}", fullClassName, assemblyName.FullName));
                Type type = Type.GetType(String.Format("{0}, {1}", fullClassName, assemblyName.FullName));
                MethodInfo methodInfo = type.GetMethod(method);
                Console.WriteLine((String)methodInfo.Invoke(null, arguments));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex is TargetParameterCountException)
                {
                    //todo
                }
            }
        }
        #endregion

        #region listProperties
        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void ListAllNamepaces(Assembly assembly)
        {
            String[] namespaceNames = assembly.GetTypes().Select(n => n.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                if (String.IsNullOrEmpty(space))
                {
                    continue;
                }
                Console.WriteLine("[N] {0}", space);
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void ListNamespaceClasses(Assembly assembly, String specificNamespace)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                if (String.IsNullOrEmpty(space) || space.ToUpper() != specificNamespace.ToUpper())
                {
                    continue;
                }
                Console.WriteLine("[N] {0}", space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == specificNamespace).Distinct().ToArray();
                    foreach (Type t in types)
                    {
                        Console.WriteLine("   [C] {0}", t.Name);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void ListClassMethods(Assembly assembly, String specificNamespace, String specificClassName)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                if (String.IsNullOrEmpty(space) || space.ToUpper() != specificNamespace.ToUpper())
                {
                    continue;
                }
                Console.WriteLine("[N] {0}", space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == specificNamespace).Distinct().ToArray();
                    foreach (Type className in types)
                    {
                        if (String.IsNullOrEmpty(className.Name) || className.Name.ToUpper() != specificClassName.ToUpper())
                        {
                            continue;
                        }
                        if (specificClassName == className.Name)
                        {
                            Console.WriteLine("   [C] {0}", className.Name);
                            foreach (MethodInfo method in className.GetMethods())
                            {
                                Console.WriteLine("      [M] {0}", method.Name);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void ListMethodParameters(Assembly assembly, String specificNamespace, String specificClassName, String specificMethodName)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                if (String.IsNullOrEmpty(space) || space.ToUpper() != specificNamespace.ToUpper())
                {
                    continue;
                }
                Console.WriteLine("[N] {0}", space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == specificNamespace).Distinct().ToArray();
                    foreach (Type className in types)
                    {
                        if (String.IsNullOrEmpty(className.Name) || className.Name.ToUpper() != specificClassName.ToUpper())
                        {
                            continue;
                        }
                        if (specificClassName == className.Name)
                        {
                            Console.WriteLine("   [C] {0}", className.Name);
                            foreach (MethodInfo method in className.GetMethods())
                            {
                                if (String.IsNullOrEmpty(method.Name) || method.Name.ToUpper() != specificMethodName.ToUpper())
                                {
                                    continue;
                                }
                                Console.WriteLine("      [M] {0}", method.Name);
                                foreach (ParameterInfo parameter in method.GetParameters())
                                {
                                    Console.WriteLine("     [P] {0,-2} {1,-15} {2}", parameter.Position, parameter.Name, parameter.ParameterType);
                                }

                                Console.WriteLine("         [R] {0,-2} {1,-15} {2}", "0", method.ReturnParameter, method.ReturnType);

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        /*
        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void ListAllClasses(Assembly assembly)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                Console.WriteLine("[N] {0}", space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == space).Distinct().ToArray();
                    foreach (Type t in types)
                    {
                        Console.WriteLine("   [C] {0}", t.Name);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        */
        /*
        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void ListAllMethods(Assembly assembly)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                Console.WriteLine("[N] {0}", space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == space).Distinct().ToArray();
                    foreach (Type className in types)
                    {
                        Console.WriteLine("\t[C] {0}", className.Name);
                        foreach (MethodInfo method in className.GetMethods())
                        {
                            Console.WriteLine("\t\t[M] {0}", method.Name);
                        }
                    }
                }
                catch(Exception error)
                {
                    Console.WriteLine(error);
                }
            }
        }
        */ 
        /*
        ////////////////////////////////////////////////////////////////////////////////
        //
        ////////////////////////////////////////////////////////////////////////////////
        private static void ListNamespaceMethods(Assembly assembly, String specificNamespace)
        {
            String[] namespaceNames = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToArray();
            foreach (String space in namespaceNames)
            {
                Console.WriteLine("[N] {0}", space);
                try
                {
                    Type[] types = assembly.GetTypes().Where(n => n.Namespace == specificNamespace).Distinct().ToArray();
                    foreach (Type className in types)
                    {
                        Console.WriteLine("\t[C] {0}", className.Name);
                        foreach (MethodInfo method in className.GetMethods())
                        {
                            Console.WriteLine("\t\t[M] {0}", method.Name);
                        }
                    }
                }
                catch(Exception error)
                {
                    Console.WriteLine(error);
                }
            }
        }
        */
        #endregion

        private static void Help(OptionSet options)
        {
            Console.WriteLine("rundotnetdll32.exe assembly.dll,class,method arguments");
            Console.WriteLine("rundotnetdll32.exe -nobanner assembly.dll,class,method arguments > output.txt");
            Console.WriteLine("rundotnetdll32.exe -list <namespaces|classes|methods> <namespace> <class>");
            return;
        }
    }
}
