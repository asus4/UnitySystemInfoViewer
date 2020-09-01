using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SystemInfoView : MonoBehaviour
{
    [SerializeField] Text label = null;
    [SerializeField] TextAsset pciDeviceTsv = null;

    private void Start()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Device Name: {SystemInfo.deviceName}");
        sb.AppendLine($"Device Model: {SystemInfo.deviceModel}");
        sb.AppendLine($"Graphics Device Vendor: {SystemInfo.graphicsDeviceVendor}");
        label.text = sb.ToString();
    }


}
