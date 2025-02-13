﻿using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Object703.Core
{
    [BurstCompile]
    [UpdateInGroup(typeof(OnPlayerInputSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial struct PlayerControlSystem : ISystem
    {
        private ComponentLookup<PlayerMoveInput> playerInputLp;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
            state.RequireForUpdate<PlayerMoveInput>();
            playerInputLp=SystemAPI.GetComponentLookup<PlayerMoveInput>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency.Complete();
            playerInputLp.Update(ref state);
            foreach (var (moveAxis,playerInput,rotateAxis) in SystemAPI
                         .Query<RefRW<MoveAxis>, RefRO<PlayerMoveInput>, RefRW<RotateAxis>>().WithAll<Simulate>().WithNone<DestructTag>())
            {   
                moveAxis.ValueRW.moveDirection = new float3(playerInput.ValueRO.leftRight, 0, playerInput.ValueRO.forwardBackward);
                rotateAxis.ValueRW.rotateEuler = new float3(0, playerInput.ValueRO.turn, 0);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        
    }
    

    // public partial struct PlayerControlJobs
    // {
    //     /// <summary>
    //     /// modify the move and rotate of player according to gathered keyboard WSAD input
    //     /// </summary>
    //     [BurstCompile]
    //     public partial struct PlayerMoveControlJob : IJobEntity
    //     {
    //         public void Execute(
    //             ref MoveAxis moveAxis,
    //             in PlayerMoveInput moveInput,
    //             ref RotateAxis rotateAxis)
    //         {
    //             moveAxis.moveDirection = new float3(moveInput.leftRight, 0, moveInput.forwardBackward);
    //             rotateAxis.rotateEuler = new float3(0, moveInput.turn, 0);
    //         }
    //     }
    //
    //     /// <summary>
    //     /// modify the skill flags of player assets according to gathered alphabet number key input
    //     /// </summary>
    //     [BurstCompile]
    //     [WithAll(typeof(PlayerAssetTag),typeof(GhostOwnerIsLocal))]
    //     public partial struct CheckSkillTriggerJob : IJobEntity
    //     {
    //         [ReadOnly]
    //         public ComponentLookup<PlayerMoveInput> inputLp;
    //         public void Execute(
    //             in Parent parent,
    //             ref SkillFlags flags)
    //         {
    //             var intput = inputLp[parent.Value];
    //             // flags.skillTriggerDown = intput.CheckPress(flags.slot);
    //         }
    //     }
    //
    //     // /// <summary>
    //     // /// update skill target according to gathered mouse input
    //     // /// </summary>
    //     // [BurstCompile]
    //     // [WithAll(typeof(PlayerAssetTag))]
    //     // public partial struct UpdateSkillCommonDataJob : IJobEntity
    //     // {
    //     //     // public NativeHashMap<int, PlayerInput> inputMap;
    //     //     [ReadOnly]
    //     //     public ComponentLookup<PlayerInput> playerInputLp;
    //     //     public void Execute(
    //     //         in GhostOwner owner,
    //     //         ref SkillCommonData commonData,
    //     //         in LocalToWorld ltw,
    //     //         in Parent parent)
    //     //     {
    //     //         if (playerInputLp.TryGetComponent(parent.Value,out var input))
    //     //         {
    //     //             commonData.target = input.mousePointEntity;
    //     //             commonData.from = ltw.Position;
    //     //             commonData.to = input.mouseWorldPoint;
    //     //             commonData.owner = parent.Value;
    //     //         }
    //     //     }
    //     // }
    //
    // }

}