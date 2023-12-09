using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDimensionChanging : IEventWithData
{
    public bool isChanging;
    public int NextLevelIndex;

    public OnDimensionChanging(bool isChanging, int nextLevelIndex)
    {
        this.isChanging = isChanging;
        NextLevelIndex = nextLevelIndex;
    }
}
