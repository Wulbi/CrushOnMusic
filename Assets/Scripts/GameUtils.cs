using System;
using BigNumber;
using UnityEngine;


public static class GameUtils
{
    public static void CalculateNumber()
    {
        //구구단을 입력
        for (int num1 = 1; num1 < 10; num1++)
        {
            for (int num2 = 1; num2 < 10; num2++)
            {
                Debug.Log($"{num1} X {num2} = {num1 * num2}");
            }
        }
    }

    public static string ConvertTime()
    {
        return "time string";
    }
}
