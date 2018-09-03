using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    //根据传入的概率数组，返回一个取值
    public static int GetRandomType(int[] RateArr)
    {
        //概率数组对应序号存储各可选类型的出现概率
        int total = 0;
        for (int i = 0; i < RateArr.Length; i++)
        {
            total += RateArr[i];
        }
        UnityEngine.Random rd = new UnityEngine.Random();
        int rad = UnityEngine.Random.Range(0, total);
        for (int i = 0; i < RateArr.Length; i++)
        {
            if (rad < RateArr[i])
            {
                return i;
            }
            else
            {
                rad -= RateArr[i];
            }
        }
        return RateArr.Length - 1;
    }

    //更新概率数组中某一个概率值
    public static void RefreshRateArr(ref int[] RateArr, int index, int cgValue)
    {
        RateArr[0] += RateArr[index] - cgValue;
        RateArr[index] = cgValue;
    }
}
