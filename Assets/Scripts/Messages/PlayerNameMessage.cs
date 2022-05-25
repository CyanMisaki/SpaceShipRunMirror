using Mirror;

namespace Messages
{
    public struct PlayerNameMessage : NetworkMessage
    {
        public NetworkConnectionToClient Conn { get; private set; }
        public string Name { get; set; }
    }
}