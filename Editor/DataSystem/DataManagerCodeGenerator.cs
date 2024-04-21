using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class DataManagerCodeGenerator
{
    public static void GenerateDataManager()
    {
        string path = "Assets/WiSdomToolSuite/Runtime/DataSystem/DataClass.cs";
        string sourceCode = File.ReadAllText(path);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = (CompilationUnitSyntax)tree.GetRoot();

        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        List<ClassInfo> classes = new List<ClassInfo>();
        foreach (var classDecl in classDeclarations)
        {
            var properties = classDecl.Members.OfType<PropertyDeclarationSyntax>()
                .Select(p => new PropertyInfo { Name = p.Identifier.ValueText, Type = p.Type.ToString() })
                .ToList();

            classes.Add(new ClassInfo { Name = classDecl.Identifier.ValueText, Properties = properties });
        }

        GenerateDataManagerClass(classes);
    }

    private static void GenerateDataManagerClass(List<ClassInfo> classes)
    {
        var filePath = "Assets/WiSdomToolSuite/Runtime/DataSystem/DataManager.cs";
        string existingCode = File.ReadAllText(filePath);

        // Create field generators from DataClass.cs
        string fieldGenerators = "";
        foreach (var classInfo in classes)
        {
            string fieldDeclaration = $"\n\t\tpublic {classInfo.Name} {classInfo.Name};";
            // Check if field already exists anywhere in the DataManager file
            if (!existingCode.Contains(classInfo.Name))
            {
                fieldGenerators += fieldDeclaration;
            }
        }

        // Append new fields at the end or a specific location if needed
        if (!string.IsNullOrEmpty(fieldGenerators))
        {
            // Example: Append directly before the class closing brace
            int insertionIndex = existingCode.IndexOf("#region Field Generators") + "#region Field Generators".Length;
            existingCode = existingCode.Insert(insertionIndex, fieldGenerators);
            File.WriteAllText(filePath, existingCode);
            Debug.Log("DataManager updated with new fields.");
        }
        else
        {
            Debug.Log("No new fields were added to DataManager.");
        }
    }

    private class ClassInfo
    {
        public string Name { get; set; }
        public List<PropertyInfo> Properties { get; set; }
    }

    private class PropertyInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
