using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TestingScaleRange : MonoBehaviour
{
    void Start()
    {
        // Linear
        float linearVal1 = UtilityFunctions.ConvertRange(10f, 20f, 0f, 1f, 15f);
        float linearExpVal1 = 0.5f;
        Assert.IsTrue(UtilityFunctions.AreFloatsClose(linearVal1, linearExpVal1), $"Expected linearVal1 to be {linearExpVal1}, not {linearVal1}");

        float linearVal2 = UtilityFunctions.ConvertRange(0f, 1f, 0f, 100f, 0.9f);
        float linearExpVal2 = 90f;
        Assert.IsTrue(UtilityFunctions.AreFloatsClose(linearVal2, linearExpVal2), $"Expected linearVal2 to be {linearExpVal2}, not {linearVal2}");

        // Reverse
        float reverseVal1 = UtilityFunctions.ConvertRange(20f, 40f, 1f, 0f, 30f);
        float reverseExpVal1 = 0.5f;
        Assert.IsTrue(UtilityFunctions.AreFloatsClose(reverseVal1, reverseExpVal1), $"Expected revVal1 to be {reverseExpVal1}, not {reverseVal1}");

        float reverseVal2 = UtilityFunctions.ConvertRange(0f, 100f, 20f, 10f, 10f);
        float reverseExpVal2 = 19f;
        Assert.IsTrue(UtilityFunctions.AreFloatsClose(reverseVal2, reverseExpVal2), $"Expected revVal1 to be {reverseExpVal2}, not {reverseVal2}");

        // Test
        //Assert.IsTrue(UtilityFunctions.AreFloatsClose(0.9f, 0.1f), "Purpose testing 1 failed");
    }
}
