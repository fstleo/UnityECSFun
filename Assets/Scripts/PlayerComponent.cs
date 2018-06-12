using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct PlayerData : IComponentData { }

public class PlayerComponent : ComponentDataWrapper<PlayerData> { }
