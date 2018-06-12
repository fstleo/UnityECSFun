using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using EntityFun.Controls;

namespace EntityFun.EntityRotation
{ 
    public class ProcessRotationInputSystem : JobComponentSystem
    {
        struct LocalRotationSpeedGroup
        {
            public ComponentDataArray<Rotation> rotations;
            [ReadOnly] public ComponentDataArray<RotationSpeedData> rotationSpeeds;
            [ReadOnly] public ComponentDataArray<RotationControlData> controlData;
            public int Length;
        }

        [Inject] private LocalRotationSpeedGroup _rotationGroup;
        [ComputeJobOptimization]
        struct RotateJob : IJobParallelFor
        {

            public ComponentDataArray<Rotation> rotations;
            [ReadOnly] public ComponentDataArray<RotationSpeedData> rotationSpeeds;
            [ReadOnly] public ComponentDataArray<RotationControlData> controlData;
            public float dt;

            public void Execute(int i)
            {
                var speed = rotationSpeeds[i].Value;
                if (speed > 0.0f)
                {
                    quaternion nRotation = math.normalize(rotations[i].Value);
                    float yaw = controlData[i].yaw * speed * dt; 
                    float pitch = controlData[i].pitch * speed * dt;
                    float roll = -controlData[i].roll * speed * dt;
                    quaternion result = math.mul(nRotation, Euler(pitch, roll, yaw));
                    rotations[i] = new Rotation
                    {
                        Value = result
                    };
                }            
            }

            quaternion Euler(float roll, float yaw, float pitch)
            {
                float cy = math.cos(yaw * 0.5f);
                float sy = math.sin(yaw * 0.5f);
                float cr = math.cos(roll * 0.5f);
                float sr = math.sin(roll * 0.5f);
                float cp = math.cos(pitch * 0.5f);
                float sp = math.sin(pitch * 0.5f);

                float qw = cy * cr * cp + sy * sr * sp;
                float qx = cy * sr * cp - sy * cr * sp;
                float qy = cy * cr * sp + sy * sr * cp;
                float qz = sy * cr * cp - cy * sr * sp;
                return new quaternion(qx, qy, qz, qw);
            }
        }        
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {       
            var job = new RotateJob
            {
                rotations = _rotationGroup.rotations,
                rotationSpeeds = _rotationGroup.rotationSpeeds,
                controlData = _rotationGroup.controlData,
                dt = Time.deltaTime
            };
		
            return job.Schedule(_rotationGroup.Length, 1, inputDeps);
        }
    }

}