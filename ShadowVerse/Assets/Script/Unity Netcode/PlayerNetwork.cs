using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{

    struct PlayerNetworkData : INetworkSerializable
    {
        private float x, y, z;
        //public bool canHit;

        internal Vector3 Position
        {
            get => new Vector3(x, y, z);
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
            serializer.SerializeValue(ref z);
            //serializer.SerializeValue(ref canHit);
        }
    }

    private readonly NetworkVariable<PlayerNetworkData> netState = new(writePerm: NetworkVariableWritePermission.Owner);
    private Vector3 storePosition;
    [SerializeField]
    private float cheapInterpolation = 0.1f;

    private void Update()
    {
        if (IsOwner)
        {
            netState.Value = new PlayerNetworkData()
            {
                Position = transform.position
            };
        }
        else
        {
            //transform.position = netState.Value.Position;
            transform.position = Vector3.SmoothDamp(transform.position, netState.Value.Position, ref storePosition, cheapInterpolation);
        }    
    }

}
