# System Info Viewer

A utility to list arbitrary properties in the [SystemInfo](https://docs.unity3d.com/ScriptReference/SystemInfo.html). May be useful for investigating platform specific issues with Unity.

![gif](https://user-images.githubusercontent.com/357497/91845465-82449a80-ec59-11ea-9479-b9f285ab4101.gif)

## How to use

1. Add the package to your project  
`https://github.com/asus4/UnitySystemInfoViewer.git?path=Packages/SystemInfoViewer#v1.1.0`

2. Put link.xml in the root of your project. and add the following code  

```xml
<linker>
    <!-- preserve UnityEngine.SystemInfo -->
    <assembly fullname="UnityEngine.CoreModule">
        <type fullname="UnityEngine.SystemInfo" preserve="all"/>
    </assembly>
</linker>
```
