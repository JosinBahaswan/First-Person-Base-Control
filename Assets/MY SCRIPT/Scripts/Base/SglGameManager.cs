using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SglGameManager : MonoBehaviour
{
    public AIHunter hunter;

    public static SglGameManager instance;

    private void Awake()
    {
        instance = this;
    }
}
