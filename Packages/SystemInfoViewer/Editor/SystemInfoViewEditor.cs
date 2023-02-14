using System.Linq;
using UnityEditor;

[CustomEditor(typeof(SystemInfoView))]
public sealed class SystemInfoViewEditor : Editor
{
    private SerializedProperty onTextChange;
    // private SerializedProperty props;
    private bool foldoutShowProps = true;
    private bool foldoutHideProps = true;
    private static string filter = "";
    private SystemInfoView _target;

    private void OnEnable()
    {
        _target = (SystemInfoView)target;
        onTextChange = serializedObject.FindProperty("onTextChange");
        // props = serializedObject.FindProperty("props");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(onTextChange);

        filter = EditorGUILayout.TextField("Filter Property", filter);

        var props = _target.Properties.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            props = props.Where(p => p.propName.ToLower().Contains(filter.ToLower()));
        }
        var showProps = props.Where(p => p.isShow);
        var hideProps = props.Where(p => !p.isShow);

        // Undo check
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // Show
            foldoutShowProps = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutShowProps, "Show Props");
            if (foldoutShowProps)
            {
                foreach (var prop in showProps)
                {
                    ShowProp(prop);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Hide
            foldoutHideProps = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutHideProps, "Hide Props");
            if (foldoutHideProps)
            {
                foreach (var prop in hideProps)
                {
                    ShowProp(prop);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (check.changed)
            {
                Undo.RecordObject(target, "Changed Property Show/Hide");
                _target.OnValidate();
            }
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
        return before != after;
    }
}
