using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SystemInfoView))]
public sealed class SystemInfoViewEditor : Editor
{
    private SerializedProperty label;
    // private SerializedProperty props;
    private bool foldoutShowProps = true;
    private bool foldoutHideProps = true;
    private static string filter = "";

    private void OnEnable()
    {
        label = serializedObject.FindProperty("label");
        // props = serializedObject.FindProperty("props");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(label);

        filter = EditorGUILayout.TextField("Filter Property", filter);

        var props = ((SystemInfoView)target).Properties.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            props = props.Where(p => p.propName.ToLower().Contains(filter.ToLower()));
        }
        var showProps = props.Where(p => p.isShow);
        var hideProps = props.Where(p => !p.isShow);

        // Undo check
        bool isPropChanged = false;

        // Show
        foldoutShowProps = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutShowProps, "Show Props");
        if (foldoutShowProps)
        {
            foreach (var prop in showProps)
            {
                isPropChanged = isPropChanged | ShowProp(prop);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Hide
        foldoutHideProps = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutHideProps, "Hide Props");
        if (foldoutHideProps)
        {
            foreach (var prop in hideProps)
            {
                isPropChanged = isPropChanged | ShowProp(prop);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (isPropChanged)
        {
            Undo.RecordObject(target, "Changed Property Show/Hide");
        }

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Show Property in Editor
    /// </summary>
    /// <param name="prop"></param>
    /// <returns>Is the property changed</returns>
    private static bool ShowProp(SystemInfoView.Prop prop)
    {
        bool before = prop.isShow;
        bool after = EditorGUILayout.ToggleLeft(prop.displayName, prop.isShow);
        prop.isShow = after;
        return before == after;
    }
}
