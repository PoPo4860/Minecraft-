using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Utile
{
    static System.Diagnostics.Stopwatch calculationTime = new System.Diagnostics.Stopwatch();

    static public void CalculationTimeStart()
    {
        calculationTime.Start();
    }
    static public void CalculationTimeStop()
    {
        calculationTime.Stop();
        Debug.Log(calculationTime.ElapsedMilliseconds.ToString() + "ms");
        calculationTime.Reset();
    }
}
