using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                        return method.Invoke(null, new object[] { });
                    }
                    else if (parameters.Length == 1 && parameters[0].ParameterType.IsEnum)
                    {
                        // Try to pass enum parameters if parameter count is 1
                        var enums = GetEnumValues(parameters[0].ParameterType);
                        var sb = new System.Text.StringBuilder();
                        sb.AppendLine();
                        foreach (var e in enums)
                        {
                            sb.AppendLine($"  - {e}: {method.Invoke(null, new object[] { e })}");
                        }
                        return sb.ToString();
                    }
                    else
                    {
                        return $"Method with parameters is not supported";
                    }
                default:
                    return $"Not Valid: {memberType}";
            }
        }

        private static IEnumerable<Enum> GetEnumValues(Type enumType)
        {
            // Get enums without Obsolete
            return Enum.GetValues(enumType).Cast<Enum>().Where(e =>
            {
                var info = e.GetType().GetField(e.ToString());
                return !IsObsolete(info);
            });
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

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = BuildInfo();
    }

    private static bool IsObsolete(MemberInfo info)
    {
        return Attribute.IsDefined(info, typeof(ObsoleteAttribute));
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
            .Where(info =>
            {
                // Filter out obsolete
                if (IsObsolete(info))
                {
                    return false;
                }
                if (info.MemberType == MemberTypes.Field || info.MemberType == MemberTypes.Property)
                {
                    return true;
                }
                if (info.MemberType == MemberTypes.Method)
                {
                    // Filter out getters
                    if (info.Name.StartsWith("get_"))
                    {
                        return false;
                    }
                    var parameters = (info as MethodInfo).GetParameters();
                    if (parameters.Length == 0)
                    {
                        return true;
                    }
                    else if (parameters.Length == 1 && parameters[0].ParameterType.IsEnum)
                    {
                        return true;
                    }
                }
                return false;
            })
            .Select(info =>
            {
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
