using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class MovementSystem : JobComponentSystem
{
    //public struct ShipsPositions
    //{
    //    public int Length;
    //    public ComponentDataArray<Position> Positions;
    //    public ComponentDataArray<Rotation> Rotations;
    //    public ComponentDataArray<SpeedData> Speeds;
    //}

    //[Inject] ShipsPositions _shipsMovementData;

    //protected override void OnUpdate()
    //{
    //    for(int i = 0; i < _shipsMovementData.Length; i++)
    //    {
    //        _shipsMovementData.Positions[i] = new Position(_shipsMovementData.Positions[i].Value + math.forward(_shipsMovementData.Rotations[i].Value) * Time.deltaTime * _shipsMovementData.Speeds[i].Value);
    //    }
    //}

    [ComputeJobOptimization]
    struct MoveShipJob : IJobProcessComponentData<Position, Rotation, SpeedData>
    {

        public float dt;

        public void Execute(ref Position position, ref Rotation rotation, ref SpeedData speed)
        {
            position.Value += math.forward(rotation.Value) * dt * speed.Value;
        }

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {               
        var job = new MoveShipJob
        {            
            dt = Time.deltaTime
        };        
        return job.Schedule(this, 64, inputDeps);
    }

}
