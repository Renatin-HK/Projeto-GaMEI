using Server;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int rodadaAtual = 0;
    public float demandaAtual;
    public int tempoPorTurno = 300;
    public bool temporizadorAtivo;
    private float tempoRestante;
    public int tempoOriginal;

    public TMP_Text textoRodada;
    public TMP_Text textoTempo;

    private float _totalDinheiroRecebido;
    private float _totalVendasRealizadas;

    public TMP_Text textoFimJogoAliado;
    public TMP_Text textoFimJogoInimigo;

    public GameObject popUpNovoTurno;
    public GameObject popUpGastos;
    public TMP_Text demandaTxt;
    public TMP_Text desafioTxt;
    public WindowsManager _windowsManager;

    [SerializeField] private IAMuitoFoda scriptIA;
    public CenarioManager cenarioManager;
    public GameManager gm;
    public TurnTimer turnTimer;
    public VendasManager vendaManager;
    [SerializeField] private PrefabSync prefabSync;

    public string descricaoDesafio;
    public TMP_Text textoGastos;
    
    public static TurnManager Instance { get; private set; } // Singleton

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        tempoOriginal = tempoPorTurno;

        cenarioManager = new CenarioManager();
        turnTimer = new TurnTimer(tempoPorTurno, AtualizarUITempo, FinalizarTurno);
        vendaManager = GetComponent<VendasManager>();
    }

    private void Update()
    {
        if (turnTimer != null && temporizadorAtivo)
        {
            turnTimer.AtualizarTempo();
        }
    }
    public void IniciarTurno()
    {
        if (rodadaAtual >= 5) // Já terminou 5 rodadas
        {
            Debug.Log("[IniciarTurno] Tentativa de iniciar turno, mas o jogo já acabou!");
            //AtualizarPontuacaoAliada();
            //AtualizarPontuacaoInimiga();
            VendasController.Instance.ResumoFinal();
            _windowsManager.MostrarTelaPontuacao();
            return; // Impede o turno de começar
        }

        Debug.Log($"[IniciarTurno] Rodada atual antes de iniciar: {rodadaAtual}");

        _windowsManager.EsconderTodasAsTelas();

        if (prefabSync != null)
        {
            prefabSync.SyncTurnData();
            prefabSync.ShowTurnUI();
            prefabSync.SyncRoundDuringTurn(); // Apenas uma vez aqui
            prefabSync.StartTurnTimer();
        }
        else
        {
            Debug.LogError("PrefabSync não está configurado no TurnManager.");
        }
    }

    
    public void ExibirPainelGastos()
    {
        popUpGastos.SetActive(true);
        textoGastos.text = $"Seus gastos este turno são:\n" +
                           $"{gm.funcionarios}x Funcionários de produção: R$ {gm.funcionarios * 1412}\n";

        gm.dinheiro -= gm.funcionarios * 1412;
        gm.dinheiro = Mathf.Max(0, gm.dinheiro);

        gm.AtualizarEstoqueUI();
    }

    public void FinalizarTurno()
    {
        if (!turnTimer.TempoEsgotado() && vendaManager.PodeEfetuarVenda())
        {
            vendaManager.EfetuarVenda();
        }

        turnTimer.tempoPorTurno = tempoOriginal;
        AvancarRodada();
    }

    public void AvancarRodada()
    {
        if (++rodadaAtual > 5)
        {
            //AtualizarPontuacaoAliada();
            //AtualizarPontuacaoInimiga();
            _windowsManager.MostrarTelaPontuacao();
        }
        else
        {
            IniciarTurno();
        }
    }

    public void AtualizarPainelDemanda()
    {
        demandaTxt.text = $"Demanda de mercado: {demandaAtual}";
        desafioTxt.text = $"Desafio: {descricaoDesafio}";
        popUpNovoTurno.SetActive(true);
    }

    public void AtualizarPontuacaoAliada()
    {
        _totalDinheiroRecebido = vendaManager.totalDinheiroRecebido;
        _totalVendasRealizadas = vendaManager.totalVendasRealizadas;

        textoFimJogoAliado.text = $"Valor recebido por vendas: R${_totalDinheiroRecebido}\n" +
                                  $"Vendas concretizadas: {_totalVendasRealizadas}\n" +
                                  $"Estoque final: {gm.estoqueProdutos}";
    }

    public void AtualizarPontuacaoInimiga()
    {
        textoFimJogoInimigo.text = $"Valor recebido por vendas: R${scriptIA.valorTotalRecebidoIA}\n" +
                                   $"Vendas concretizadas: {scriptIA.finalVendasIA}\n" +
                                   $"Estoque final: {scriptIA.estoqueIA}";
    }

    public void AtualizarUITempo(float minutos, float segundos)
    {
        textoTempo.text = $"{minutos:00}:{segundos:00}";
    }

    public void AtualizarUIRodada()
    {
        textoRodada.text = $"{rodadaAtual}/5";
    }

    public void ConfirmarDemanda()
    {
        popUpNovoTurno.SetActive(false);
        ExibirPainelGastos();
    }

    public void ConfirmarGastos()
    {
        popUpGastos.SetActive(false);
    }

    /*
    public void ResetVariaveis()
    {
        //Variáveis da classe GameManager
            //Estoque
        gm.dinheiro = gm.dinheiroInicial;
        gm.estoqueMalhaPV = 0;
        gm.estoqueElastico = 0;
        gm.estoqueEmbalagens = 0;
        gm.estoqueEtiquetas = 0;
        gm.estoqueLinha = 0;
        
            //Preço
        gm.precoMalhaPV = 10.84f;
        gm.precoElastico = 0.13f;
        gm.precoEmbalagens = 0.01f;
        gm.precoEtiquetas = 1;
        gm.precoLinha = 2.35f;

        gm.totalVendas = 0;
        gm.funcionarios = 0;
        gm.estoqueProdutos = 0;
        
        //Variáveis da classe TurnManager
        rodadaAtual = 0;
        tempoPorTurno = tempoOriginal;
        
        
        //Variáveis da classe VendasManager
        vendaManager.valorVendaNoTurno = 0;
        vendaManager.vendasRealizadas = 0;
        vendaManager.totalVendasRealizadas = 0;
        vendaManager.totalDinheiroRecebido = 0;
        
        //Variáveis da classe IAMuitoFoda
        scriptIA.producaoIA = 0;
        scriptIA.estoqueIA = 0;
        scriptIA.vendasTotaisIA = 0;
        scriptIA.valorTotalRecebidoIA = 0;
        scriptIA.finalVendasIA = 0;
        
        gm.AtualizarEstoqueUI();
        _windowsManager.EsconderTodasAsTelas();
        vendaManager.ZerarInputs();
        
        IniciarTurno();
    }
    */
}
