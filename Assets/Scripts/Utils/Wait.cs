using System.Collections.Generic;
using UnityEngine;

// 코루틴의 WaitForSeconds를 캐싱하는 유틸
public static class Wait
{
    private static readonly Dictionary<float, WaitForSeconds> _waits = new();

    public static WaitForSeconds For(float seconds)
    {
        if (!_waits.TryGetValue(seconds, out var wait))
        {
            wait = new WaitForSeconds(seconds);
            _waits[seconds] = wait;
        }
        return wait;
    }
}