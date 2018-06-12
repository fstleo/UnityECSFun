using System;
using Unity.Entities;

namespace EntityFun.EntityRotation
{
    [Serializable]
    public struct RotationSpeedData : IComponentData
    {
        public int Value;
    }

    public class RotationSpeedComponent : ComponentDataWrapper<RotationSpeedData> { }
}