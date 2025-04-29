using UnityEngine;
using UnityEngine.UI;

public class WindowsManager : MonoBehaviour
{
    [Header("Telas")]
    [SerializeField] private GameObject telaChat;
    [SerializeField] private GameObject telaCompras;
    [SerializeField] private GameObject telaEstoque;
    [SerializeField] private GameObject telaProd;
    [SerializeField] private GameObject telaVendas;
    [SerializeField] private GameObject telaPontuacao;
    [SerializeField] private GameObject telaGastos;
    [SerializeField] private GameObject telaInfo;

    [System.Serializable]
    public class BotaoTela
    {
        public Button botao;
        public Sprite normalSprite;
        public Sprite selectedSprite;
    }

    [Header("Botões")]
    [SerializeField] private BotaoTela botaoChat;
    [SerializeField] private BotaoTela botaoCompras;
    [SerializeField] private BotaoTela botaoEstoque;
    [SerializeField] private BotaoTela botaoProd;
    [SerializeField] private BotaoTela botaoVendas;
    [SerializeField] private BotaoTela botaoPontuacao;

    /// <summary>
    /// Ativa uma tela específica e atualiza o visual do botão correspondente
    /// </summary>
    private void MostrarTela(GameObject tela, BotaoTela botaoSelecionado)
    {
        EsconderTodasAsTelas();
        ResetarSpritesBotoes();

        if (tela != null) 
            tela.SetActive(true);

        if (botaoSelecionado != null)
        {
            botaoSelecionado.botao.image.sprite = botaoSelecionado.selectedSprite;
        }
    }

    /// <summary>
    /// Desativa todas as telas gerenciadas
    /// </summary>
    public void EsconderTodasAsTelas()
    {
        telaChat.SetActive(false);
        telaCompras.SetActive(false);
        telaEstoque.SetActive(false);
        telaProd.SetActive(false);
        telaVendas.SetActive(false);
        telaPontuacao.SetActive(false);
        telaGastos.SetActive(false);
        telaInfo.SetActive(false);
    }

    private void ResetarSpritesBotoes()
    {
        botaoChat.botao.image.sprite = botaoChat.normalSprite;
        botaoCompras.botao.image.sprite = botaoCompras.normalSprite;
        botaoEstoque.botao.image.sprite = botaoEstoque.normalSprite;
        botaoProd.botao.image.sprite = botaoProd.normalSprite;
        botaoVendas.botao.image.sprite = botaoVendas.normalSprite;
    }

    #region Métodos Públicos para UI
    // Os métodos abaixo são chamados pelos botões na interface

    public void MostrarTelaChat() => MostrarTela(telaChat, botaoChat);
    public void MostrarTelaCompras() => MostrarTela(telaCompras, botaoCompras);
    public void MostrarTelaEstoque() => MostrarTela(telaEstoque, botaoEstoque);
    public void MostrarTelaProd() => MostrarTela(telaProd, botaoProd);
    public void MostrarTelaVendas() => MostrarTela(telaVendas, botaoVendas);
    public void MostrarTelaPontuacao() => MostrarTela(telaPontuacao, null);

    #endregion
}