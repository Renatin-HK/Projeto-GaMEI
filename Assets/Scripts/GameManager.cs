using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Dinheiro e estoque inicial
    public float dinheiro;
    public float dinheiroInicial = 10000;
    public float estoqueMalhaPV = 0;
    public float estoqueElastico = 0;
    public float estoqueEmbalagens = 0;
    public float estoqueEtiquetas = 0;
    public float estoqueLinha = 0;
    public float estoqueProdutos = 0;
    
    
    // Preço base dos materiais
    public float precoMalhaPV = 10.84f;
    public float precoElastico = 0.13f;
    public float precoEmbalagens = 0.01f;
    public float precoEtiquetas = 1;
    public float precoLinha = 2.35f;

    // Consumo por produto
    private float consumoMalhaPV = 0.06f;
    private float consumoElastico = 1.6f;
    private float consumoEmbalagens = 1;
    private float consumoEtiquetas = 1;
    private float consumoLinha = 0.01f;

    // Referências para InputFields de quantidade (definido na UI)
    public TMP_InputField inputQuantidadeMalha;
    public TMP_InputField inputQuantidadeElastico;
    public TMP_InputField inputQuantidadeEmbalagens;
    public TMP_InputField inputQuantidadeEtiquetas;
    public TMP_InputField inputQuantidadeLinha;
    
    //Referência da quantidade a ser produzida
    public TMP_InputField inputQuantidadeProduzir;

    // Referências para os textos de UI
    //public TMP_Text telaPontuacao;
    
    //Referência para os botões
    public Button botaoMalhaPV;
    public Button botaoElastico;
    public Button botaoEmbalagens;
    public Button botaoEtiquetas;
    public Button botaoLinha;
    public Button botaoProducao;
    public Button botaoFuncionarioProducao;
    
    // Referências para os textos de preço
    public TMP_Text textoPrecoMalhaPV;
    public TMP_Text textoPrecoElastico;
    public TMP_Text textoPrecoEmbalagens;
    public TMP_Text textoPrecoEtiquetas;
    public TMP_Text textoPrecoLinha;
    
    public TMP_Text textoDinheiro;
    public TMP_Text textoMateriasPrimas;
    public TMP_Text textoProdutosFuncionarios;

    
    public float totalVendas;
    private bool canClick;
    private Color btnOgColor;
    public int funcionarios = 1;

    public GameObject popUpErroProd;
    public GameObject popUpInfoFuncionarios;


    private NetworkManager netManager;
    public float produtosProduzidos;
    
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        netManager = NetworkManager.Singleton;
        dinheiro = dinheiroInicial;
        btnOgColor = Color.white;
        canClick = true;
       AtualizarEstoqueUI();
       AtualizarPrecos();
    }
    
    public void BotaoComprarMalhaPV()
    {
        int quantidade = int.Parse(inputQuantidadeMalha.text); 
        BotaoComprar("MalhaPV", quantidade, precoMalhaPV);
    }

    public void BotaoComprarElastico()
    {
        if (canClick)
        {
            canClick = false;
            int quantidade = int.Parse(inputQuantidadeElastico.text); 
            BotaoComprar("Elastico", quantidade, precoElastico);
        }
  
    }

    public void BotaoComprarEmbalagens()
    {
        if (canClick)
        {
            canClick = false;
            int quantidade = int.Parse(inputQuantidadeEmbalagens.text); 
            BotaoComprar("Embalagens", quantidade, precoEmbalagens);
    }
        }

    public void BotaoComprarEtiquetas()
    {
        if (canClick)
        {
            canClick = false;
            
            int quantidade = int.Parse(inputQuantidadeEtiquetas.text);
            BotaoComprar("Etiquetas", quantidade, precoEtiquetas);
        }
    }

    public void BotaoComprarLinha()
    {
        if (canClick)
        {
            canClick = false;
            int quantidade = int.Parse(inputQuantidadeLinha.text); 
            BotaoComprar("Linha", quantidade, precoLinha);
        }
    }

    // Função genérica de compra que é chamada pelas funções específicas de cada material
    public void BotaoComprar(string tipoMaterial, int quantidade, float precoUnitario)
    {
        Button botaoAtual = null; // Variável para armazenar o botão atual

        // Define qual botão chamar para o efeito
        switch (tipoMaterial)
        {
            case "MalhaPV":
                botaoAtual = botaoMalhaPV;
                break;
            case "Elastico":
                botaoAtual = botaoElastico;
                break;
            case "Embalagens":
                botaoAtual = botaoEmbalagens;
                break;
            case "Etiquetas":
                botaoAtual = botaoEtiquetas;
                break;
            case "Linha":
                botaoAtual = botaoLinha;
                break;
            default:
                Debug.Log("Tipo de material inválido!");
                return;
        }
        
        // Verifica se o dinheiro é suficiente
        if (dinheiro >= precoUnitario * quantidade)
        {
            // Lógica de compra (permanece igual)
            switch (tipoMaterial)
            {
                case "MalhaPV":
                    estoqueMalhaPV += quantidade;
                    break;
                case "Elastico":
                    estoqueElastico += quantidade;
                    break;
                case "Embalagens":
                    estoqueEmbalagens += quantidade;
                    break;
                case "Etiquetas":
                    estoqueEtiquetas += quantidade;
                    break;
                case "Linha":
                    estoqueLinha += quantidade;
                    break;
            }
            dinheiro -= precoUnitario * quantidade;
            AtualizarEstoqueUI();
            EfeitoSucesso(botaoAtual.image);
        }
        else
        {
            EfeitoFalha(botaoAtual.image);
            Debug.Log("Dinheiro insuficiente!");

        }
    }

    public void BotaoContratarFuncionarioProd()
    {
        if (canClick)
        {
            canClick = false;
            funcionarios++;
            AtualizarEstoqueUI();
            EfeitoSucesso(botaoFuncionarioProducao.image);
        }
        else
        {
            //EfeitoFalha(botaoFuncionarioProducao.image);
        }
        
    }
    
    // Método para alterar preços
    public void AlterarPrecoMateriais(float percentual)
    {
        precoMalhaPV *= (1 + percentual);
        precoElastico *= (1 + percentual);
        precoEmbalagens *= (1 + percentual);
        precoEtiquetas *= (1 + percentual);
        precoLinha *= (1 + percentual);
        
        Debug.Log("Preços dos materiais atualizados.");
    }
    
    public void AtualizarPrecos()
    {
        textoPrecoMalhaPV.text = $"R$ {precoMalhaPV:F2} / kg"; // Atualiza o texto com o preço formatado
        textoPrecoElastico.text = $"R$ {precoElastico:F2} / m";
        textoPrecoEmbalagens.text = $"R$ {precoEmbalagens:F2} / un";
        textoPrecoEtiquetas.text = $"R$ {precoEtiquetas:F2} / un";
        textoPrecoLinha.text = $"R$ {precoLinha:F2} / m";
    }
    
    public void AtualizarEstoqueUI()
    {
        // Exibição do dinheiro (Coluna 1)
        textoDinheiro.text = $"Dinheiro: R${dinheiro.ToString("F2")}";

        // Exibição das matérias-primas (Coluna 2)
        string materiasPrimas = $"Malha PV: {estoqueMalhaPV.ToString("F2")} kg\n" +
                                $"Elástico: {estoqueElastico.ToString("F2")} m\n" +
                                $"Embalagens: {estoqueEmbalagens} un\n" +
                                $"Etiquetas: {estoqueEtiquetas} un\n" +
                                $"Linha: {estoqueLinha.ToString("F2")} m";

        // Exibição dos produtos e funcionários (Coluna 3)
        string produtosEFuncionarios = $"Produtos: {estoqueProdutos}\nFuncionários: {funcionarios}";

        // Atualizando os textos das colunas
        textoMateriasPrimas.text = materiasPrimas;      // Coluna 2
        textoProdutosFuncionarios.text = produtosEFuncionarios;  // Coluna 3
    }
    
    public void BotaoProduzir()
    {
        int quantidade = int.Parse(inputQuantidadeProduzir.text);
        int capacidadeMaxima = funcionarios * 10;

        if (quantidade > capacidadeMaxima)
        {
            canClick = false;
            EfeitoFalha(botaoProducao.image);
            popUpErroProd.SetActive(true);
            return;
        }

        if (!VerificarMateriasPrimasNecessarias(quantidade))
        {
            canClick = false;
            EfeitoFalha(botaoProducao.image);
            return;
        }

        DeduzirMateriasPrimas(quantidade);

        estoqueProdutos += quantidade;
        produtosProduzidos = quantidade;

        // Avisa o Host sobre o novo estoque (se for Client)
        if (!NetworkManager.Singleton.IsHost)
        {
            AtualizarEstoqueNoHostServerRpc(estoqueProdutos);
        }

        AtualizarEstoqueUI();
        EfeitoSucesso(botaoProducao.image); 
    }

    // Verifica se há matérias-primas suficientes
    private bool VerificarMateriasPrimasNecessarias(int quantidade)
    {
        float malhaNecessaria = consumoMalhaPV * quantidade;
        float elasticoNecessario = consumoElastico * quantidade;
        float embalagensNecessarias = consumoEmbalagens * quantidade;
        float etiquetasNecessarias = consumoEtiquetas * quantidade;
        float linhaNecessaria = consumoLinha * quantidade;

        return estoqueMalhaPV >= malhaNecessaria &&
               estoqueElastico >= elasticoNecessario &&
               estoqueEmbalagens >= embalagensNecessarias &&
               estoqueEtiquetas >= etiquetasNecessarias &&
               estoqueLinha >= linhaNecessaria;
    }

// Deduz as matérias-primas do estoque
    private void DeduzirMateriasPrimas(int quantidade)
    {
        estoqueMalhaPV -= consumoMalhaPV * quantidade;
        estoqueElastico -= consumoElastico * quantidade;
        estoqueEmbalagens -= consumoEmbalagens * quantidade;
        estoqueEtiquetas -= consumoEtiquetas * quantidade;
        estoqueLinha -= consumoLinha * quantidade;
    }
    
    public void AtualizarEstoqueNoHost(float novoEstoque)
    {
        // Verifica se é um Client (não Host)
        if (netManager != null && netManager.IsClient && !netManager.IsHost)
        {
            AtualizarEstoqueNoHostServerRpc(novoEstoque);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AtualizarEstoqueNoHostServerRpc(float novoEstoque)
    {
        estoqueProdutos = novoEstoque;
        AtualizarEstoqueUI(); // Isso garante que o Host também veja a mudança
        //AtualizarEstoqueNoClientRpc(novoEstoque); // Opcional, para refletir no Client
    }

    [ClientRpc]
    public void AtualizarEstoqueNoClientRpc(float novoEstoque)
    {
        estoqueProdutos = novoEstoque;
        AtualizarEstoqueUI();
    }

    
    /*  VVVVVVVVVVVVVVVVVVVVVV
     *  SÓ SCRIPT DE UI DAQUI PRA BAIXO
     *  VVVVVVVVVVVVVVVVVVVVVV
     */
    private void EfeitoSucesso(Image botao)
    {
        Color originalColor = btnOgColor; // Salva a cor original do botão

        LeanTween.color(botao.rectTransform, Color.green, 0.2f)
            .setLoopPingPong(1)
            .setOnComplete(() =>
            {
                botao.color = originalColor; // Restaura a cor original
                canClick = true; // Permite o clique novamente somente após a animação
            });
    }

    private void EfeitoFalha(Image botao)
    {
        Vector3 originalPosition = botao.transform.localPosition;
        Color originalColor = btnOgColor; // Salva a cor original do botão
            
        // Animação de tremer e piscar
        LeanTween.moveLocalX(botao.gameObject, originalPosition.x - 5f, 0.05f)
            .setLoopPingPong(3)
            .setOnComplete(() =>
            {
                botao.transform.localPosition = originalPosition; // Volta para a posição original
            });

        LeanTween.color(botao.rectTransform, Color.red, 0.1f)
            .setLoopPingPong(2)
            .setOnComplete(() =>
            {
                botao.color = originalColor; // Restaura a cor original
                canClick = true; // Permite o clique novamente após a animação
            });
    }

    public void ConfirmarPopUps()
    {
        popUpErroProd.SetActive(false);
        popUpInfoFuncionarios.SetActive(false);
    }

    public void ExibirInfo()
    {
        popUpInfoFuncionarios.SetActive(true);
    }

}
