using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using EntityFun.Spawn;
using EntityFun.Utils;

namespace EntityFun.Spawn
{
    public class SpawnRandomInSphereSystem : ComponentSystem
    {
        private static Dictionary<int, List<Entity>> ships = new Dictionary<int, List<Entity>>();

        struct SpawnRandomInSphereInstance
        {
            public int spawnerIndex;
            public Entity sourceEntity;
            public float3 position;
            public float radius;
        }

        ComponentGroup m_MainGroup;

        protected override void OnCreateManager(int capacity)
        {
            m_MainGroup = GetComponentGroup(typeof(SpawnRandomInSphere), typeof(Position));
        }

        protected override void OnUpdate()
        {
            var uniqueTypes = new List<SpawnRandomInSphere>(1);

            EntityManager.GetAllUniqueSharedComponentDatas(uniqueTypes);

            int spawnInstanceCount = 0;
            for (int sharedIndex = 0; sharedIndex != uniqueTypes.Count; sharedIndex++)
            {
                var spawner = uniqueTypes[sharedIndex];
                m_MainGroup.SetFilter(spawner);
                var entities = m_MainGroup.GetEntityArray();
                spawnInstanceCount += entities.Length;
            }

            if (spawnInstanceCount == 0)
                return;

            var spawnInstances = new NativeArray<SpawnRandomInSphereInstance>(spawnInstanceCount, Allocator.Temp);
            {
                int spawnIndex = 0;
                for (int sharedIndex = 0; sharedIndex != uniqueTypes.Count; sharedIndex++)
                {
                    var spawner = uniqueTypes[sharedIndex];
                    m_MainGroup.SetFilter(spawner);
                    var entities = m_MainGroup.GetEntityArray();
                    var positions = m_MainGroup.GetComponentDataArray<Position>();

                    for (int entityIndex = 0; entityIndex < entities.Length; entityIndex++)
                    {
                        var spawnInstance = new SpawnRandomInSphereInstance();

                        spawnInstance.sourceEntity = entities[entityIndex];
                        spawnInstance.spawnerIndex = sharedIndex;
                        spawnInstance.position = positions[entityIndex].Value;

                        spawnInstances[spawnIndex] = spawnInstance;
                        spawnIndex++;
                    }
                }
            }

            for (int spawnIndex = 0; spawnIndex < spawnInstances.Length; spawnIndex++)
            {
                int spawnerIndex = spawnInstances[spawnIndex].spawnerIndex;
                var spawner = uniqueTypes[spawnerIndex];
                int count = spawner.count;
                var entities = new NativeArray<Entity>(count, Allocator.Temp);
                var prefab = spawner.prefab;
                float radius = spawner.radius;
                var spawnPositions = new NativeArray<float3>(count, Allocator.Temp);
                float3 center = spawnInstances[spawnIndex].position;
                var sourceEntity = spawnInstances[spawnIndex].sourceEntity;

                GeneratePoints.RandomPointsInSphere(center, radius, ref spawnPositions);

                EntityManager.Instantiate(prefab, entities);
                ships[spawnIndex] = new List<Entity>();
                for (int i = 0; i < count; i++)
                {
                    var position = new Position
                    {
                        Value = spawnPositions[i]
                    };
                    EntityManager.SetComponentData(entities[i], position);
                    EntityManager.SetComponentData(entities[i], new SpeedData { Value = Random.Range(15, 25) });
                    ships[spawnIndex].Add(entities[i]);
                }

                EntityManager.RemoveComponent<SpawnRandomInSphere>(sourceEntity);

                spawnPositions.Dispose();
                entities.Dispose();
            }
            spawnInstances.Dispose();
        }
    }
}