using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System.Net;
using System;
using System.Collections.Generic;

public struct DiscoveryRequest : NetworkMessage { }

public struct DiscoveryResponse : NetworkMessage
{
    public long serverId;
    public string roomCode; // 5자리 코드
    public Uri uri;
}

public class RoomDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    public static RoomDiscovery Instance { get; private set; }
    private void Awake() => Instance = this;

    protected override DiscoveryRequest GetRequest() => new DiscoveryRequest();

    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        return new DiscoveryResponse
        {
            serverId = ServerId,
            roomCode = ServerCodeGenerator.ToRoomCode(ServerId),
            uri = transport.ServerUri()
        };
    }

    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        string ip = endpoint.Address.ToString();
        RoomDirectory.Instance.Register(response.roomCode, ip);
    }
}
