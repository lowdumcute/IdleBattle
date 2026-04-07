using Fusion;

public struct NetworkCharacterData : INetworkStruct
{
    public NetworkString<_16> characterID;
    public int level;
    public int star;
    public int realm;
    public int position;

}