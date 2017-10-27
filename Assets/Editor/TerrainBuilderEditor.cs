using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*Este script es solo para crear un boton para ejecutar el script de pintar terreno sin tener que darle al play*/
[CustomEditor(typeof(PaintTerrain))]
public class TerrainBuilderEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PaintTerrain myScript = (PaintTerrain)target;
        if(GUILayout.Button("Generate Terrain"))
        {
            myScript.Start();
        }
    }
}
