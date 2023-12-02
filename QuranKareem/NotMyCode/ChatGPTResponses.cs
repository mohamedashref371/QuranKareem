using System;
using System.Reflection;

class ChatGPTResponses // GPT-3.5
{
    public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        // Specify the path to the folder where the DLL is located
        string folderPath = @"libraries";

        // Construct the full path to the DLL
        string assemblyPath = System.IO.Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");

        // Check if the DLL exists at the specified path
        if (System.IO.File.Exists(assemblyPath))
        {
            // Load and return the assembly from the specified path
            return Assembly.LoadFrom(assemblyPath);
        }

        // Return null if the DLL is not found
        return null;
    }

}
