using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[Serializable]
public struct SpeedData : IComponentData
{
    public int Value;
}

public class SpeedComponent : ComponentDataWrapper<SpeedData> {}
