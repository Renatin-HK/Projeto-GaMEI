using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Server
{
    public class NetworkConnectionManager : MonoBehaviour
    {
        [SerializeField] private GameObject errorPanel; // Painel de erro para mensagens.
        [SerializeField] private TMP_Text errorMessage; // Texto do painel (TextMeshPro).
        [SerializeField] private GameObject menu; // Menu inicial.
        [SerializeField] private GameObject gamePanels; // Paineis do jogo.
        [SerializeField] private GameObject timer; // Temporizador do jogo.
        [SerializeField] private GameObject gameStartManagerPrefab; // Prefab do GameStartManager.

        private void Start()
        {
            // Inicializa com o menu visível e elementos do jogo ocultos.
            menu.SetActive(true);
            gamePanels.SetActive(false);
            timer.SetActive(false);

            // Garante que o painel de erro está inicialmente desativado.
            errorPanel.SetActive(false);
        }
        private void OnEnable()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnDisable()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }


        // Cria o host
        public void StartHost()
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
            {
                // Já existe um host ou cliente ativo.
                ShowTemporaryError("Host já existente, conecte-se como client.");
                return;
            }

            if (NetworkManager.Singleton.StartHost())
            {
                //Debug.Log("Host criado com sucesso!");
                TransitionToGame();
                SpawnGameStartManager(); // Spawn no host.
            }
            else
            {
                Debug.LogError("Falha ao criar o host.");
                ShowTemporaryError("Não foi possível criar o host.");
            }
        }
/*
        public void StartClient()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                ShowTemporaryError("Você já é o host. Não pode conectar como cliente.");
                return;
            }

            if (NetworkManager.Singleton.IsClient)
            {
                ShowTemporaryError("Você já está conectado como cliente.");
                return;
            }

            if (!NetworkManager.Singleton.StartClient())
            {
                ShowTemporaryError("Host não disponível.");
                return;
            }

            // Inscreve-se nos callbacks para monitorar a conexão.
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            StartCoroutine(TimeoutClientConnection(1f)); // Timeout de 3 segundos para conexão.
        }
        // Callback para quando o cliente conectar com sucesso
        private void OnClientConnected(ulong clientId)
        {
            //Debug.Log($"Cliente conectado com sucesso! ID: {clientId}");
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            StopAllCoroutines(); // Cancela o timeout, pois a conexão foi bem-sucedida.
            TransitionToGame(); // Inicia o jogo para o cliente.
        }
*/

        private void StartClient()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                ShowTemporaryError("Você já é o host. Não pode conectar como cliente.");
                return;
            }

            if (NetworkManager.Singleton.IsClient)
            {
                ShowTemporaryError("Você já está conectado como cliente.");
                return;
            }

            if (!NetworkManager.Singleton.StartClient())
            {
                ShowTemporaryError("Host não disponível.");
                return;
            }

            StartCoroutine(TimeoutClientConnection(3f));
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"Cliente conectado com sucesso! ID: {clientId}");
            TransitionToGame(); // Transição para o painel do jogo
        }

        // Callback para quando o cliente não conseguir se conectar
        private void OnClientDisconnected(ulong clientId)
        {
            Debug.LogError($"Cliente {clientId} foi desconectado. Verifique se o host está ativo ou se houve um problema na conexão.");
            ShowTemporaryError("Não foi possível se conectar. Verifique se o host está ativo.");
            NetworkManager.Singleton.Shutdown(); // Desconecta o cliente.
        }

        // Timeout para tentativa de conexão como cliente
        private IEnumerator TimeoutClientConnection(float timeout)
        {
            yield return new WaitForSeconds(timeout);

            if (!NetworkManager.Singleton.IsConnectedClient)
            {
                Debug.LogWarning("Tempo limite de conexão atingido. Cancelando tentativa de cliente.");
                NetworkManager.Singleton.Shutdown(); // Cancela a tentativa de conexão.
                ShowTemporaryError("Host não disponível.");
            }
        }

        // Faz a transição do menu para o jogo
        private void TransitionToGame()
        {
            //Debug.Log("Transição para o jogo iniciada.");
            menu.SetActive(false);
            gamePanels.SetActive(true);
            timer.SetActive(true);
        }

        // Spawn do GameStartManager no host
        private void SpawnGameStartManager()
        {
            if (gameStartManagerPrefab == null)
            {
                Debug.LogError("Prefab do GameStartManager não atribuído.");
                return;
            }

            // Instancia e spawna o objeto na rede
            GameObject gameStartManagerInstance = Instantiate(gameStartManagerPrefab);
            gameStartManagerInstance.GetComponent<NetworkObject>().Spawn();
        }

        // Mostra uma mensagem de erro no painel por 3 segundos
        private void ShowTemporaryError(string message)
        {
            StopAllCoroutines(); // Para garantir que nenhuma mensagem anterior esteja ativa.
            errorPanel.SetActive(true);
            errorMessage.text = message;
            StartCoroutine(HideErrorAfterDelay(3f));
        }

        // Coroutine para esconder o painel de erro após 3 segundos
        private IEnumerator HideErrorAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            errorPanel.SetActive(false);
        }
    }
}
