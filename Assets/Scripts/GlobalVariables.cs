using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables
{
    private static float sfxVolume = 1, musicVolume = 1, masterVolume = 1;

    public static float SfxVolume
    {
        get
        {
            return sfxVolume;
        }
        set
        {
            if (value >= 0 && value <= 1)
            {
                sfxVolume = value;
            }
        }
    }

    public static float MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            if (value >= 0 && value <= 1)
            {
                musicVolume = value;
            }
        }
    }
    public static float MasterVolume
    {
        get
        {
            return masterVolume;
        }
        set
        {
            if (value >= 0 && value <= 1)
            {
                masterVolume = value;
            }
        }
    }
}
