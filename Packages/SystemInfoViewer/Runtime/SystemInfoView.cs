using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Show selected list of SystemInfo values
/// https://docs.unity3d.com/ScriptReference/SystemInfo.html
/// </summary>

[ExecuteInEditMode]
public sealed class SystemInfoView : MonoBehaviour
{
    [Serializable]
    public class TextChangeEvent : UnityEngine.Events.UnityEvent<string> { }

    [Serializable]
    public class Prop
    {
        public string propName;
        public string displayName;
        public bool isShow;
        public MemberTypes memberType;

        public bool IsValid => !string.IsNullOrWhiteSpace(propName)
                            && !string.IsNullOrWhiteSpace(displayName);


        public object GetValue()
        {
            if (string.IsNullOrWhiteSpace(propName))
            {
                return $"Not Valid";
            }

            var type = typeof(SystemInfo);
            switch (memberType)
            {
                case MemberTypes.Field:
                    return type.GetField(propName).GetValue(null);
                case MemberTypes.Property:
                    return type.GetProperty(propName).GetValue(null);
                case MemberTypes.Method:
                    var method = type.GetMethod(propName);
                    if (method.GetParameters().Length == 0)
                    {
                        return method.Invoke(null, new object[] { });
                    }
                    else
                    {
                        return $"Method with Parameter is not supported";
                    }
            }

            return $"Not Valid: {memberType}";
        }
    }

    [SerializeField] private Prop[] props;
    [SerializeField] private TextChangeEvent onTextChange = new TextChangeEvent();

    public Prop[] Properties => props;

    private int lastPropCount;

    private void Start()
    {
        string text = BuildInfo();
        onTextChange?.Invoke(text);
        Debug.Log($"[SystemInfo]:\n{text}");
    }

    public string BuildInfo()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var prop in props)
        {
            if (prop.isShow)
            {
                sb.AppendLine($"- {prop.displayName}: {prop.GetValue()}");
            }
        }
        return sb.ToString();
    }


    #region Editor Code
#if UNITY_EDITOR
    public void OnValidate()
    {
        if (props == null
            || props.Length == 0
            || props.Any(p => !p.IsValid))
        {
            RebuildProps();
            return;
        }

        onTextChange?.Invoke(BuildInfo());
        var count = props.Sum(p => p.isShow ? 1 : 0);
        if (lastPropCount == count)
        {
            return;
        }
        lastPropCount = count;
    }

    private void RebuildProps()
    {
        Debug.Log("Rebuild Props");
        props = typeof(SystemInfo)
            .GetMembers(BindingFlags.Public | BindingFlags.Static)
            .Where(info => info.MemberType == MemberTypes.Field
                || info.MemberType == MemberTypes.Property)
            .Select(info =>
            {
                Debug.Log($"{info} MemberType: {info.MemberType}");
                return new Prop()
                {
                    propName = info.Name,
                    displayName = ObjectNames.NicifyVariableName(info.Name),
                    isShow = false,
                    memberType = info.MemberType,
                };
            }).ToArray();
    }
#endif // UNITY_EDITOR
    #endregion // Editor Code

}
