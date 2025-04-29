using UnityEngine;
using UnityEngine.UI;

public class WindowsManager : MonoBehaviour
{
    // Referências para as telas
    [SerializeField] private GameObject telaChat;
    [SerializeField] private GameObject telaCompras;
    [SerializeField] private GameObject telaEstoque;
    [SerializeField] private GameObject telaProd;
    [SerializeField] private GameObject telaVendas;
    [SerializeField] private GameObject telaPontuacao;
    [SerializeField] private GameObject telaGastos;
    [SerializeField] private GameObject telaInfo;

    // Estrutura para armazenar botões e seus sprites
    [System.Serializable]
    public class BotaoTela
    {
        public Button botao;           // O botão da tela
        public Sprite normalSprite;    // Sprite para o estado não selecionado
        public Sprite selectedSprite;  // Sprite para o estado selecionado
    }

    // Referências para os botões e seus sprites
    [SerializeField] private BotaoTela botaoChat;
    [SerializeField] private BotaoTela botaoCompras;
    [SerializeField] private BotaoTela botaoEstoque;
    [SerializeField] private BotaoTela botaoProd;
    [SerializeField] private BotaoTela botaoVendas;
    [SerializeField] private BotaoTela botaoPontuacao;

    // Método para mostrar uma tela específica e esconder as outras
    private void MostrarTela(GameObject tela, BotaoTela botaoSelecionado)
    {
        // Esconde todas as telas primeiro
        EsconderTodasAsTelas();

        // Reseta todos os botões para o sprite normal
        ResetarSpritesBotoes();

        // Mostra apenas a tela especificada
        if (tela != null) tela.SetActive(true);

        // Muda o sprite do botão selecionado
        if (botaoSelecionado != null)
        {
            botaoSelecionado.botao.image.sprite = botaoSelecionado.selectedSprite;
        }
    }

    // Método para esconder todas as telas
    public void EsconderTodasAsTelas()
    {
        telaChat.SetActive(false);
        telaCompras.SetActive(false);
        telaEstoque.SetActive(false);
        telaProd.SetActive(false);
        telaVendas.SetActive(false);
        telaPontuacao.SetActive(false);
    }

    // Método para resetar o sprite de todos os botões para o estado normal
    private void ResetarSpritesBotoes()
    {
        botaoChat.botao.image.sprite = botaoChat.normalSprite;
        botaoCompras.botao.image.sprite = botaoCompras.normalSprite;
        botaoEstoque.botao.image.sprite = botaoEstoque.normalSprite;
        botaoProd.botao.image.sprite = botaoProd.normalSprite;
        botaoVendas.botao.image.sprite = botaoVendas.normalSprite;
    }

    // Métodos para os botões que chamam MostrarTela com a tela e o botão desejados
    public void MostrarTelaChat()
    {
        MostrarTela(telaChat, botaoChat);
    }

    public void MostrarTelaCompras()
    {
        MostrarTela(telaCompras, botaoCompras);
    }

    public void MostrarTelaEstoque()
    {
        MostrarTela(telaEstoque, botaoEstoque);
    }

    public void MostrarTelaProd()
    {
        MostrarTela(telaProd, botaoProd);
    }

    public void MostrarTelaVendas()
    {
        MostrarTela(telaVendas, botaoVendas);
    }

    public void MostrarTelaPontuacao()
    {
        MostrarTela(telaPontuacao, null);
    }
}
