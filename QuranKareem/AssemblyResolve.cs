﻿using System;
using System.Reflection;

class AssemblyResolve // ChatGPT-3.5 Coding
{
    public static void AssemblyResolveEventHandler() => AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        // Specify the path to the folder where the DLL is located
        string folderPath = "libraries";

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
