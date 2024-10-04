using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// object to use in the material diciotnary
public struct matChecker
{
    public matChecker(Material MAT, bool CLEAR)
    {
        mat = MAT;
        clear = CLEAR;
    }

    public Material mat { get; }
    public bool clear { get; }

    public override string ToString() => $"matBuffer (\nMaterial: {mat} \nShould be Clear?: {clear}\n)";
}
