//----------------------------------------------
// DynamoDB Helper
// Copyright © 2014-2015 OuijaPaw Games LLC
//----------------------------------------------

using UnityEngine;
using UnityEditor;

namespace DDBHelper
{
    [InitializeOnLoad]
    public static class DDBWindow
    {
        [MenuItem("Window/DDBHelper", false, 10000)]
        public static void ShowWindow()
        {
            EditorUtility.DisplayDialog("DynamoDB Helper",
                                        "Version 2.0\n" +
                                        "Copyright © 2014-15 OuijaPaw Games LLC\n" +
                                        "ouijapaw@gmail.com",
                                        "Ok"); 
        }
    }
}