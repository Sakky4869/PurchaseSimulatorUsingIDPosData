﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// あ
/// </summary>
public class ScreenSizeAdjuster : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(1600, 900, false);
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }
}
