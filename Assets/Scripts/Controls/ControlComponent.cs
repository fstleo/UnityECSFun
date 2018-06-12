using Unity.Entities;

namespace EntityFun.Controls
{
    public struct RotationControlData : IComponentData
    {
        public float roll;
        public float pitch;
        public float yaw;
    }

    public class ControlComponent : ComponentDataWrapper<RotationControlData> { }
}