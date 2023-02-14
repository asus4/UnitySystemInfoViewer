# System Info Viewer

A utility to list arbitrary properties in the [SystemInfo](https://docs.unity3d.com/ScriptReference/SystemInfo.html). May be useful for investigating platform specific issues with Unity.

https://user-images.githubusercontent.com/357497/218889056-bf58afe7-2686-4fbf-90e5-630b437dd40f.mp4

## How to use

1. Add the package to your project  
`https://github.com/asus4/UnitySystemInfoViewer.git?path=Packages/SystemInfoViewer#v1.1.1`

2. Put `link.xml` in the root of your project and add the following code if your project uses code-stripping

```xml
<linker>
    <!-- preserve UnityEngine.SystemInfo -->
    <assembly fullname="UnityEngine.CoreModule">
        <type fullname="UnityEngine.SystemInfo" preserve="all"/>
    </assembly>
</linker>
```

3. Attach the `SystemInfoView` component to your GameObject
4. Select properties you want to test
