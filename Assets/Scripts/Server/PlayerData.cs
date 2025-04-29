using Unity.Netcode;

namespace Server
{
    public class PlayerData : NetworkBehaviour
    {
        public NetworkVariable<float> valorRecebido = new NetworkVariable<float>();
        public NetworkVariable<float> vendasRealizadas = new NetworkVariable<float>();
        public NetworkVariable<float> estoqueFinal = new NetworkVariable<float>();
    }
}