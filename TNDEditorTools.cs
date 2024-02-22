using System.Linq;
using UnityEditor;
using UnityEngine;

public class TNDEditorTools : EditorWindow
{
    [MenuItem("TND/Fix Missing Scripts")]
    static void Init() {
        TNDEditorTools window = (TNDEditorTools)EditorWindow.GetWindow(typeof(TNDEditorTools));
        window.Show();
    }

    void OnGUI() {
        if(GUILayout.Button("1. Find all Missing Scripts")) {
            FixMissingScript();
        }
        if(GUILayout.Button("2. Find and Replace Broken Materials")) {
            FindBrokenMaterials();
        }
    }

    public static void FixMissingScript() {
        string[] prefabPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab")).ToArray();

        for(int i = 0; i < prefabPaths.Length; i++) {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPaths[i]);
            Clean(prefab.transform);
        }
    }

    public static void Clean(Transform obj) {
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj.gameObject);

        //Recursive remove missing scripts
        for(int i = 0; i < obj.childCount; i++) {
            Clean(obj.GetChild(i));
        }
    }


    public static void FindBrokenMaterials() {
        Shader _newShader = Shader.Find("Standard");
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/" });

        for(int i = 0; i < guids.Length; i++) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            Material _mat = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material)) as Material; 
            if(_mat.shader.name.Contains("Hidden/InternalErrorShader")) {
                _mat.shader = _newShader;
            }
        }
    }
}

