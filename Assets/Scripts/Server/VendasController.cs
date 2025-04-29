using System;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class VendasController : NetworkBehaviour
{
    // UI Elements
    [SerializeField] private TMP_InputField inputQuantidade;
    [SerializeField] private TMP_InputField inputPreco;
    [SerializeField] private TextMeshProUGUI textoResultados;
    [SerializeField] private GameObject telaResultados;
    [SerializeField] private GameObject telaAguarde;

    // Game Managers
    [SerializeField] private GameManager gm;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private WindowsManager windowsManager;
    public static VendasController Instance { get; private set; }

    // Inputs e Recursos
    private float hostQuantidade, hostPreco, clientQuantidade, clientPreco;
    public float estoqueHost, estoqueClient, dinheiroHost, dinheiroClient;
    [SerializeField]private bool hostConfirmado, clientConfirmado, hostProntoParaNovoTurno, clientProntoParaNovoTurno;
    
    private float hostVendido;
    private float clientVendido;
    
    // Variáveis para acumular os resultados finais
    private float totalVendasHost = 0f;
    private float totalVendasClient = 0f;
    private float totalProdutosHost = 0f;
    private float totalProdutosClient = 0f;
    
    // Referências para o painel final do jogo
    [SerializeField] private TMP_Text painelHostText;
    [SerializeField] private TMP_Text painelClientText;
    [SerializeField] private GameObject painelResumoFinal;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void DebugButtonClick()
    {
        CapturarInputs();
    }
    public void ConfirmarResumo()
    {
        ConfirmarNovoTurnoServerRpc();
    }
    private void TelaPopUp(GameObject tela, bool state)
    {
        tela.SetActive(state);
    }
    
    [ClientRpc]
    private void EsconderTelasClientRpc()
    {
        TelaPopUp(telaAguarde, false);
        TelaPopUp(telaResultados, false);
    }


    private void SincronizarRecursosLocais()
    {
        if (IsHost)
        {
            estoqueHost = gm.estoqueProdutos;
            dinheiroHost = gm.dinheiro;
        }
        else
        {
            estoqueClient = gm.estoqueProdutos;
            dinheiroClient = gm.dinheiro;
        }
    }

    private void CapturarInputs()
    {
        SincronizarRecursosLocais();
        AtualizarEstoqueServerRpc(gm.estoqueProdutos);
        

        Debug.Log("Capturando os inputs...");

        float quantidade = ObterQuantidadeInput();
        float preco = ObterPrecoInput();

        if (IsHost)
        {
            hostQuantidade = Mathf.Min(quantidade, estoqueHost);
            hostPreco = preco;
            ConfirmarInputServerRpc(hostQuantidade, hostPreco, true);
        }
        else
        {
            clientQuantidade = Mathf.Min(quantidade, estoqueClient);
            clientPreco = preco;
            ConfirmarInputServerRpc(clientQuantidade, clientPreco, false);
        }

        TelaPopUp(telaAguarde, true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ConfirmarInputServerRpc(float quantidade, float preco, bool isHost, ServerRpcParams serverRpcParams = default)
    {
        if (isHost)
        {
            hostQuantidade = quantidade;
            hostPreco = preco;
            hostConfirmado = true;
        }
        else
        {
            clientQuantidade = quantidade;
            clientPreco = preco;
            clientConfirmado = true;
        }

        AtualizarConfirmacoesClientRpc(hostConfirmado, clientConfirmado);

        if (hostConfirmado && clientConfirmado)
        {
            CalcularVendasServerRpc();
        }
    }

    [ClientRpc]
    private void AtualizarConfirmacoesClientRpc(bool hostConfirmado, bool clientConfirmado)
    {
        this.hostConfirmado = hostConfirmado;
        this.clientConfirmado = clientConfirmado;
    }

    private float ObterQuantidadeInput()
    {
        if (float.TryParse(inputQuantidade.text, out float quantidade))
        {
            return quantidade;
        }

        Debug.LogError("Erro ao capturar input de quantidade!");
        return 0f;
    }

    private float ObterPrecoInput()
    {
        if (float.TryParse(inputPreco.text, out float preco))
        {
            return preco;
        }
        Debug.LogError("Erro ao capturar input de preço!");
        return 0f;
    }


    [ServerRpc]
    private void CalcularVendasServerRpc()
    {
        telaAguarde.SetActive(false);

        float demandaRestante = turnManager.demandaAtual;
        float lucroHost = 0f, lucroClient = 0f;

        if (hostPreco < clientPreco || (hostPreco == clientPreco && IsHost))
        {
            hostVendido = Mathf.Min(hostQuantidade, demandaRestante);
            demandaRestante -= hostVendido;
            lucroHost = hostVendido * hostPreco;

            clientVendido = Mathf.Min(clientQuantidade, demandaRestante);
            demandaRestante -= clientVendido;
            lucroClient = clientVendido * clientPreco;

            AtualizarRecursos(hostVendido, clientVendido, lucroHost, lucroClient);
            AtualizarGMClientRpc(gm.estoqueProdutos, gm.dinheiro);
            
            // Atualizar os totais
            totalVendasHost += lucroHost;
            totalProdutosHost += hostVendido;

            totalVendasClient += lucroClient;
            totalProdutosClient += clientVendido;


        }
        else
        {
            clientVendido = Mathf.Min(clientQuantidade, demandaRestante);
            demandaRestante -= clientVendido;
            lucroClient = clientVendido * clientPreco;

            hostVendido = Mathf.Min(hostQuantidade, demandaRestante);
            demandaRestante -= hostVendido;
            lucroHost = hostVendido * hostPreco;

            AtualizarRecursos(hostVendido, clientVendido, lucroHost, lucroClient);
            AtualizarGMClientRpc(gm.estoqueProdutos, gm.dinheiro);
            
            // Atualizar os totais
            totalVendasHost += lucroHost;
            totalProdutosHost += hostVendido;

            totalVendasClient += lucroClient;
            totalProdutosClient += clientVendido;
        }
        AtualizarResultadosClientRpc(hostVendido, clientVendido, lucroHost, lucroClient);


    }

    private void AtualizarRecursos(float hostVendido, float clientVendido, float lucroHost, float lucroClient)
    {
        estoqueHost -= hostVendido;
        estoqueClient -= clientVendido;

        dinheiroHost += lucroHost;
        dinheiroClient += lucroClient;

        if (IsHost)
        {
            gm.estoqueProdutos = estoqueHost;
            gm.dinheiro = dinheiroHost;
        }
        else
        {
            gm.estoqueProdutos = estoqueClient;
            gm.dinheiro = dinheiroClient;
        }
    }


    [ClientRpc]
    private void AtualizarResultadosClientRpc(float hostVendas, float clientVendas, float lucroHost, float lucroClient)
    {
        float vendas = IsHost ? hostVendas : clientVendas;
        float lucro = IsHost ? lucroHost : lucroClient;
        float vendasAdversario = IsHost ? clientVendas : hostVendas;

        string mensagem = $"Você vendeu: {vendas} produtos\n";
        mensagem += $"Lucro: R$ {lucro:0.00}\n";
        mensagem += $"Produtos vendidos pelo adversário: {vendasAdversario}";

        textoResultados.text = mensagem;
        telaResultados.SetActive(true);
        
    }

    [ClientRpc]
    private void AtualizarGMClientRpc(float estoque, float dinheiro)
    {
        gm.estoqueProdutos = estoque;
        gm.dinheiro = dinheiro;

        if (IsHost)
        {
            estoqueHost = estoque;
            dinheiroHost = dinheiro;
        }
        else
        {
            estoqueClient = estoque;
            dinheiroClient = dinheiro;
        }
    }




    [ServerRpc(RequireOwnership = false)]
    private void ConfirmarNovoTurnoServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log($"[ServerRpc] Chamado por client ID: {senderClientId}");

        if (senderClientId == 1)
        {
            Debug.Log("entrou no if do cliente, É CLIENTE");
            clientProntoParaNovoTurno = true;
        }

        if (senderClientId == 0)
        {
            Debug.Log("entrou no if do host, É HOST");
            hostProntoParaNovoTurno = true;
        }

        AtualizarProntosClientRpc(hostProntoParaNovoTurno, clientProntoParaNovoTurno);

        // Aqui você pode checar se ambos confirmaram
        if (hostProntoParaNovoTurno && clientProntoParaNovoTurno)
        {
            Debug.Log("Ambos os jogadores confirmaram. Iniciando novo turno...");
            EsconderTelasClientRpc();
            IniciarNovoTurnoServerRpc();
        }
    }
    
    [ClientRpc]
    private void AtualizarProntosClientRpc(bool hostProntoParaNovoTurno, bool clientProntoParaNovoTurno)
    {
        this.hostProntoParaNovoTurno = hostProntoParaNovoTurno;
        this.clientProntoParaNovoTurno = clientProntoParaNovoTurno;
    }
    [ServerRpc(RequireOwnership = false)]
    private void AtualizarEstoqueServerRpc(float estoqueAtualizado, ServerRpcParams serverRpcParams = default)
    {
        if (IsHost) return; // Host não precisa fazer nada aqui
    
        estoqueClient = estoqueAtualizado;
    }


    [ServerRpc]
    private void IniciarNovoTurnoServerRpc()
    {
        hostProntoParaNovoTurno = false;
        clientProntoParaNovoTurno = false;
        hostConfirmado = false;
        clientConfirmado = false;
        Debug.Log("Iniciando novo turno...");
        turnManager.IniciarTurno();
    }
    
    [ClientRpc]
    public void MostrarResumoFinalClientRpc(float totalVendasHost, float totalVendasClient, float totalProdutosHost, float totalProdutosClient, float estoqueHost, float estoqueClient)
    {
        Debug.Log("Atualizando painel final");

        string resumoHost = $"Valor Recebido: R$ {totalVendasHost:0.00}\n" +
                            $"Vendas Concretizadas: {totalProdutosHost}\n" +
                            $"Estoque Final: {estoqueHost}";

        string resumoClient = $"Valor Recebido: R$ {totalVendasClient:0.00}\n" +
                              $"Vendas Concretizadas: {totalProdutosClient}\n" +
                              $"Estoque Final: {estoqueClient}";

        painelHostText.text = resumoHost;
        painelClientText.text = resumoClient;

        painelResumoFinal.SetActive(true);
    }

    public void ResumoFinal()
    {
        if (IsHost)
        {
            // Host já tem os dados sincronizados
            MostrarResumoFinalClientRpc(
                totalVendasHost,
                totalVendasClient,
                totalProdutosHost,
                totalProdutosClient,
                estoqueHost,
                estoqueClient // Agora está correto!
            );
        }
        else
        {
            // Client envia seu estoque FINAL para o Host
            EnviarEstoqueFinalServerRpc(GameManager.Instance.estoqueProdutos);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnviarEstoqueFinalServerRpc(float estoqueFinalClient)
    {
        estoqueClient = estoqueFinalClient;
        MostrarResumoFinalClientRpc(
            totalVendasHost,
            totalVendasClient,
            totalProdutosHost,
            totalProdutosClient,
            estoqueHost,
            estoqueClient
        );
    }


}
