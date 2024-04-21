using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WiSdomTool
{
    [MenuItem("WiSdom/Generate Code")]
    private static void GenerateCodeMenuItem()
    {
        CodeGenerator.GenerateClass();
    }
    [MenuItem("WiSdom/Generate DataManager")]
    public static void GenerateDataManager()
    {
        DataManagerCodeGenerator.GenerateDataManager();
    }
}
