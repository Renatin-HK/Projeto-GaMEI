using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

namespace Server
{


    public class GameStartManager : NetworkBehaviour
    {
        [SerializeField] private TMP_Text waitingMessage;
        [SerializeField] private GameObject waitingPanel;
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private TMP_Text errorMessage;

        [SerializeField] private GameObject menu; // Menu inicial.
        [SerializeField] private GameObject gamePanels; // Paineis do jogo.
        [SerializeField] private GameObject timer; // Temporizador do jogo.

        [SerializeField] private PrefabSync prefabSync;

        private bool gameStarted = false;

        private void Start()
        {
            // Inicializa com o menu visível e elementos do jogo ocultos.
            menu.SetActive(true);
            gamePanels.SetActive(false);
            timer.SetActive(false);

            // Garante que o painel de erro está inicialmente desativado.
            errorPanel.SetActive(false);
        }

        public void StartAsHost()
        {
            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("Host iniciado com sucesso.");
                TransitionToGame();
                waitingPanel.SetActive(true);
                waitingMessage.text = "Aguardando jogadores...";
            }
            else
            {
                ShowError("Erro ao iniciar como host. Verifique sua conexão.");
            }
        }

        public void StartAsClient()
        {
            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Cliente tentando conectar ao host...");
                TransitionToGame();
                waitingPanel.SetActive(true);
                waitingMessage.text = "Conectando ao host...";
            }
            else
            {
                ShowError("Erro ao conectar ao host. Verifique sua conexão.");
            }
        }

        private void ShowError(string message)
        {
            Debug.LogError(message);
            errorMessage.text = message;
            errorPanel.SetActive(true);
        }

        private void HideError()
        {
            errorPanel.SetActive(false);
        }

        public override void OnNetworkSpawn()
        {
            if (IsHost)
            {
                Debug.Log("Host inicializado. Aguardando conexões.");
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"Cliente conectado com sucesso! ID: {clientId}");

            if (IsHost)
            {
                int connectedClients = NetworkManager.Singleton.ConnectedClients.Count;
                Debug.Log($"Clientes conectados: {connectedClients}");

                if (connectedClients >= 2 && !gameStarted)
                {
                    Debug.Log("Todos os jogadores conectados. Iniciando contagem regressiva...");
                    gameStarted = true;
                    StartCountdownClientRpc();
                }
            }
        }

        public void StartGame()
        {
            if (!IsHost) return;

            Debug.Log("Iniciando o turno no host.");
          prefabSync.StartGame();
        }


        private void TransitionToGame()
        {
            //Debug.Log("Transição para o jogo iniciada.");
            menu.SetActive(false);
            gamePanels.SetActive(true);
            timer.SetActive(true);
        }

        [ClientRpc]
        private void StartCountdownClientRpc()
        {
            Debug.Log("Iniciando contagem regressiva no cliente.");


            // Chama o temporizador na interface (incluir lógica de exibição da contagem)
            timer.SetActive(true);
            StartCoroutine(CountdownRoutine());
        }

        private IEnumerator CountdownRoutine()
        {
            int countdownTime = 3; // Por exemplo, 5 segundos de contagem regressiva.
            while (countdownTime > 0)
            {
                Debug.Log($"Contagem regressiva: {countdownTime}");
                waitingMessage.text = $"Começando em {countdownTime}...";
                yield return new WaitForSeconds(1);
                countdownTime--;
            }

            // Notificar que a contagem terminou e iniciar o turno
            waitingMessage.text = "Aguarde.";
            waitingPanel.SetActive(false);
            StartGame();
        }



        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
    }
}