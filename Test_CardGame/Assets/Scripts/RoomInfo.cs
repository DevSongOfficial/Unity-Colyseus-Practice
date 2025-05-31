using System.Collections.Generic;


[System.Serializable]
public class RoomInfo
{
    public string roomId;
    public string name;
    public int clients;
    public int maxClients;
}

[System.Serializable]
public class RoomListWrapper
{
    public List<RoomInfo> rooms;
}