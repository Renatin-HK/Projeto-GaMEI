using System;
using UnityEngine;

public class IAMuitoFoda : MonoBehaviour
{
    public int producaoIA = 0; // Total de produtos produzidos pela IA
    public float estoqueIA = 0; // Estoque atual da IA
    public float vendasTotaisIA = 0; // Total de vendas feitas pela IA
    public float valorTotalRecebidoIA = 0f; // Valor total recebido pela IA nas vendas
    private float variacaoPreco = 0f; // Variação de preço (-10% a +10%)
    private float precoFinalIA = 0f; // Preço final da IA para o turno
    private int quantidadeParaVenderIA = 0; // Quantidade que a IA decidiu vender no turno
    private float producaoMultiplicador = 1f; // Fator que diminui a chance da IA produzir mais que o jogador

    public float finalVendasIA;

    // Método para imitar a produção do jogador, com variação de -20% a +20%
    public void ImitarProducao(int producaoJogador)
    {
        // Variação aleatória entre -20% e +20% da produção do jogador
        float variacao = UnityEngine.Random.Range(-0.2f, 0.1f);
        int producaoImitada = Mathf.CeilToInt(producaoJogador * (1 + variacao) * producaoMultiplicador);

        // Limita a produção para evitar que a IA tenha estoque excessivo
        producaoImitada = Mathf.Max(0, producaoImitada);

        producaoIA += producaoImitada;
        estoqueIA += producaoImitada;

        // Diminui o multiplicador para evitar que a IA continue produzindo muito mais que o jogador
        if (producaoImitada > producaoJogador)
        {
            producaoMultiplicador *= 0.85f; // Reduz a chance de produzir mais
        }

        Debug.Log($"IA produziu: {producaoImitada} produtos. Estoque IA: {estoqueIA}");
    }

    // Método para imitar a venda do jogador, com variação de -20% a +20%
    public void ImitarVenda(float precoJogador, int quantidadeJogador, float demandaAtual)
    {
        // Preço da IA com variação de -10% a +10% em relação ao preço do jogador
        variacaoPreco = UnityEngine.Random.Range(-0.1f, 0.1f);
        precoFinalIA = Mathf.Round(precoJogador * (1 + variacaoPreco) * 100f) / 100f;

        // Quantidade a vender com variação de -20% a +20%, respeitando o estoque e a demanda
        float variacaoQuantidade = UnityEngine.Random.Range(-0.2f, 0.2f);
        quantidadeParaVenderIA = Mathf.Min(Mathf.CeilToInt(quantidadeJogador * (1 + variacaoQuantidade)), (int)estoqueIA, (int)demandaAtual);

        // Não altera o estoque nem o valor total recebido aqui
    }

    // Atualiza o estoque da IA
    public void AtualizarEstoqueIA(float quantidadeVendida)
    {
        estoqueIA -= quantidadeVendida;
        Debug.Log($"Estoque da IA atualizado. Estoque restante: {estoqueIA}");
    }

    // Métodos Get para obter informações
    public float GetPrecoIA() => precoFinalIA;
    public int GetQuantidadeIA() => quantidadeParaVenderIA;
}
