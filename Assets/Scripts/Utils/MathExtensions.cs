using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class MathExtensions
{
    public static bool SafeComparison(float a, float b)
    {
        return Math.Abs(a - b) < 0.1f;
    }
}

