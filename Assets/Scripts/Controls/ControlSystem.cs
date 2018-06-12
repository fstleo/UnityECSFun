using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using EntityFun.EntityRotation;

namespace EntityFun.Controls
{
    public class UserControlSystem : ComponentSystem
    {
        public struct InputPlayerData
        {
            public int Length;
            [ReadOnly] public ComponentDataArray<PlayerData> Data;
            public ComponentDataArray<RotationControlData> Controls;
        }

        [Inject] InputPlayerData _playerData;

        protected override void OnUpdate()
        {
            for (int i = 0; i < _playerData.Length; i++)
            {
                _playerData.Controls[i] = new RotationControlData
                {
                    roll = Input.GetAxis("Horizontal"),
                    pitch = Input.GetAxis("Vertical"),
                    yaw = Input.GetKey(KeyCode.Q) ? -1 : Input.GetKey(KeyCode.E) ? 1 : 0
                };
            }
        }
    }
}