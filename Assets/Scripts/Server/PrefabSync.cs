using Unity.Netcode;
using UnityEngine;

namespace Server
{
    public class PrefabSync : NetworkBehaviour
    {
        [SerializeField] private TurnManager turnManager;

        public void StartGame()
        {
            if (!IsHost) return;

            //Debug.Log("Iniciando o jogo após a sincronização.");
            turnManager.IniciarTurno();
            ShowTurnUI();
            StartTurnTimer();
        }
/*
        public void SyncTurnData()
        {
            if (!IsHost) return;

            turnManager.cenarioManager.SortearCenario(turnManager);
            string descricaoCenario = turnManager.cenarioManager.ObterDescricaoCenario();
            float demandaAtual = Random.Range(10, 20);

            int tempoPorTurno = turnManager.tempoOriginal;
            if (descricaoCenario.Contains("tempo deste turno"))
            {
                tempoPorTurno = 180;
                turnManager.tempoPorTurno = tempoPorTurno;
            }

            if (descricaoCenario.Contains("teve um aumento de"))
            {
                SyncPrecos();
            }

            turnManager.demandaAtual = demandaAtual;
            turnManager.descricaoDesafio = descricaoCenario;

            SyncTurnDataClientRpc(descricaoCenario, demandaAtual);
            SyncTurnTimeClientRpc(tempoPorTurno);
        }
*/
        public void SyncTurnData()
        {
            if (!IsHost)
            {
                Debug.Log("[SyncTurnData] Ignorado no cliente.");
                return;
            }

            Debug.Log($"[SyncTurnData] Executado no host. NetworkObjectId: {NetworkObjectId}");

            // Sorteio no host
            turnManager.cenarioManager.SortearCenario(turnManager);
            string descricaoCenario = turnManager.cenarioManager.ObterDescricaoCenario();
            float demandaAtual = Random.Range(10, 20);

            Debug.Log($"[SyncTurnData] Cenário sorteado: {descricaoCenario}. Demanda: {demandaAtual}");
            
            int tempoPorTurno = turnManager.tempoOriginal;
            if (descricaoCenario.Contains("tempo deste turno"))
            {
                tempoPorTurno = 180;
                turnManager.tempoPorTurno = tempoPorTurno;
            }

            // Caso o cenário altere preços
            if (descricaoCenario.Contains("teve um aumento de"))
            {
                Debug.Log("[SyncTurnData] Aplicando aumento de preços...");
                turnManager.gm.AlterarPrecoMateriais(0.2f); // Aumenta os preços no host
                SyncPrecos(); // Sincroniza os preços para os clientes
            }

            // Atualiza localmente no host
            turnManager.demandaAtual = demandaAtual;
            turnManager.descricaoDesafio = descricaoCenario;

            // Sincroniza com os clientes
            SyncTurnDataClientRpc(descricaoCenario, demandaAtual);
            SyncTurnTimeClientRpc(tempoPorTurno);
        }


        [ClientRpc]
        private void SyncTurnDataClientRpc(string descricaoCenario, float demandaAtual)
        {
            turnManager.demandaAtual = demandaAtual;
            turnManager.descricaoDesafio = descricaoCenario;
            turnManager.AtualizarPainelDemanda();
        }

        public void ShowTurnUI()
        {
            if (!IsHost) return;

            turnManager.AtualizarPainelDemanda();
            ShowTurnUIClientRpc();
        }

        [ClientRpc]
        private void ShowTurnUIClientRpc()
        {
            turnManager.AtualizarPainelDemanda();
        }

        public void SyncRodadaAtual()
        {
            if (!IsHost) return;

            SyncRodadaAtualClientRpc(turnManager.rodadaAtual);
        }

        [ClientRpc]
        private void SyncRodadaAtualClientRpc(int rodada)
        {
            turnManager.rodadaAtual = rodada;
            turnManager.AtualizarUIRodada();
        }

        public void SyncPrecos()
        {
            if (!IsHost) return;
            
            SyncPrecosClientRpc(
                turnManager.gm.precoMalhaPV,
                turnManager.gm.precoElastico,
                turnManager.gm.precoEmbalagens,
                turnManager.gm.precoEtiquetas,
                turnManager.gm.precoLinha
            );

            turnManager.gm.AtualizarPrecos();
        }

        [ClientRpc]
        private void SyncPrecosClientRpc(float malhaPV, float elastico, float embalagens, float etiquetas, float linha)
        {
            turnManager.gm.precoMalhaPV = malhaPV;
            turnManager.gm.precoElastico = elastico;
            turnManager.gm.precoEmbalagens = embalagens;
            turnManager.gm.precoEtiquetas = etiquetas;
            turnManager.gm.precoLinha = linha;

            turnManager.gm.AtualizarPrecos();
        }

        public void StartTurnTimer()
        {
            if (!IsHost) return;

            //Debug.Log("Iniciando temporizador no host.");
            turnManager.turnTimer.ResetarTempo();
            turnManager.temporizadorAtivo = true;

            StartTurnTimerClientRpc();
        }

        [ClientRpc]
        private void StartTurnTimerClientRpc()
        {
            //Debug.Log("Iniciando temporizador no cliente.");
            turnManager.turnTimer.ResetarTempo();
            turnManager.temporizadorAtivo = true;
        }

        [ClientRpc]
        private void SyncTurnTimeClientRpc(int tempoPorTurno)
        {
            turnManager.tempoPorTurno = tempoPorTurno;
            turnManager.turnTimer.tempoPorTurno = tempoPorTurno;

            turnManager.AtualizarUITempo(tempoPorTurno / 60, tempoPorTurno % 60);
        }
        
        public void SyncInitialRound()
        {
            if (!IsHost) return;

            Debug.Log($"[SyncInitialRound] Rodada inicial no host: {turnManager.rodadaAtual}");
            SyncInitialRoundClientRpc(turnManager.rodadaAtual);
        }

        [ClientRpc]
        private void SyncInitialRoundClientRpc(int rodadaInicial)
        {
            Debug.Log($"[SyncInitialRoundClientRpc] Cliente recebendo rodada inicial: {rodadaInicial}");
            turnManager.rodadaAtual = rodadaInicial;
            turnManager.AtualizarUIRodada();
        }

        public void SyncRoundDuringTurn()
        {
            if (!IsHost) return;

            Debug.Log($"[SyncRoundDuringTurn] Rodada antes de incrementar: {turnManager.rodadaAtual}");

            // Incrementa apenas uma vez
            turnManager.rodadaAtual += 1;

            Debug.Log($"[SyncRoundDuringTurn] Nova rodada no host: {turnManager.rodadaAtual}");

            // Sincroniza com os clientes
            SyncRoundDuringTurnClientRpc(turnManager.rodadaAtual);
        }

        [ClientRpc]
        private void SyncRoundDuringTurnClientRpc(int novaRodada)
        {
            Debug.Log($"[SyncRoundDuringTurnClientRpc] Cliente recebendo nova rodada: {novaRodada}");

            turnManager.rodadaAtual = novaRodada;
            turnManager.AtualizarUIRodada();
        }





        public void ConfirmarTurno()
        {
            if (!IsHost) return;

            turnManager.ConfirmarDemanda();
            StartTurnTimer();
        }
    }
}
