using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace rundotnetdll32
{
    class Interactive
    {
        AppDomain appDomain = null;
        Assembly assembly = null;
        Boolean execute = true;

        String namespaceName = String.Empty;
        String className = String.Empty;
        String methodName = String.Empty;

        OrderedDictionary parameters;

        internal Interactive(String file)
        {
            assembly = Assembly.LoadFile(file);
        }

        internal void Execute()
        { 
            while (execute)
            {
                var prompt = new StringBuilder();
                prompt.Append(assembly.GetName().Name);
                if (!String.IsNullOrEmpty(namespaceName))
                {
                    prompt.Append(@"\" + namespaceName);
                    if (!String.IsNullOrEmpty(className))
                    {
                        prompt.Append(@"\" + className);
                        if (!String.IsNullOrEmpty(methodName))
                        {
                            prompt.Append(@"\" + methodName);
                        }
                    }
                }

                Console.Write("({0}) > ", prompt.ToString());
                String input = Console.ReadLine();
                String action = NextItem(ref input).ToLower().Trim();
                switch (action)
                {
                    case ("execute"):
                        Type type = assembly.GetType(namespaceName + "." + className);
                        MethodInfo methodInfo = type.GetMethod(methodName);
                        Object[] args = new Object[parameters.Count];
                        for (Int32 i = 0; i < parameters.Count; i++)
                            args[i] = parameters[i];
                        Console.WriteLine((String)methodInfo.Invoke(null, args));
                        break;
                    case ("options"):
                        Options(input);
                        break;
                    case ("use"):
                        Use(input);
                        break;
                    case ("set"):
                        Set(input);
                        break;
                    case ("exit"):
                        execute = false;
                        break;
                    default:
                        Console.WriteLine("Unknown Option");
                        Console.WriteLine("use set options execute\n");
                        break;
                }
            }
        }

        private void Use(String input)
        {
            String action = NextItem(ref input).ToLower().Trim();
            switch (action)
            {
                case ("namespace"):
                    namespaceName = input;
                    className = String.Empty;
                    methodName = String.Empty;
                    Console.WriteLine("Namespace => {0}\n", namespaceName);
                    break;
                case ("class"):
                    className = input;
                    methodName = String.Empty;
                    Console.WriteLine("Class => {0}\n", className);
                    break;
                case ("method"):
                    methodName = input;
                    Console.WriteLine("Method => {0}\n", methodName);
                    parameters = new OrderedDictionary();
                    foreach (ParameterInfo p in assembly.GetTypes()
                        .Where(n => n.Namespace == namespaceName)
                        .Where(c => c.Name == className)
                        .FirstOrDefault().GetMethods()
                        .Where(m => m.Name == methodName)
                        .FirstOrDefault().GetParameters())
                    {
                        parameters.Add(p.Name, p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null);
                    }
                    break;
                default:
                    Console.WriteLine("Unknown Option for Use Command");
                    Console.WriteLine("use (namespace|class|method) name\n");
                    break;
            }
        }

        private void Options(String input)
        {
            if (!String.IsNullOrEmpty(namespaceName))
            {
                if (!String.IsNullOrEmpty(className))
                {
                    if (!String.IsNullOrEmpty(methodName))
                    {
                        foreach (ParameterInfo p in assembly.GetTypes()
                            .Where(n => n.Namespace == namespaceName)
                            .Where(c => c.Name == className)
                            .FirstOrDefault().GetMethods()
                            .Where(m => m.Name == methodName)
                            .FirstOrDefault().GetParameters())
                        {
                            Console.WriteLine("[P] {0,-2} {1,-15} {2}", p.Position, p.Name, p.ParameterType);
                        }
                    }
                    else
                    {
                        foreach (MethodInfo m in assembly.GetTypes()
                            .Where(n => n.Namespace == namespaceName)
                            .Where(c => c.Name == className)
                            .FirstOrDefault().GetMethods())
                        {
                            Console.WriteLine("[M] {0}", m.Name);
                        }
                    }
                }
                else
                {
                    foreach (Type t in assembly.GetTypes().Where(n => n.Namespace == namespaceName).Distinct().ToArray())
                        Console.WriteLine("[C] {0}", t.Name);
                }
            }
            else
            {
                foreach (String s in assembly.GetTypes().Select(n => n.Namespace).Distinct().ToArray())
                    Console.WriteLine("[N] {0}", s);
            }
        }

        private void Set(String input)
        {
            String key = NextItem(ref input);
            if (parameters.Contains(key))
            {
                parameters[key] = input;
                Console.WriteLine("{0} => {1}", key, input);
            }
            else
            {
                Console.WriteLine("Invalid Key");
            }
        }

        public static String NextItem(ref String input)
        {
            String option = String.Empty;
            String[] options = input.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (options.Length > 1)
            {
                option = options[0];
                input = String.Join(" ", options, 1, options.Length - 1);
            }
            else
            {
                option = input;
            }
            return option.ToLower();
        }
    }
}
