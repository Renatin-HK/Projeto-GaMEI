using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VendasManager : MonoBehaviour
{
    public TurnManager turnManager;
    
    // Referências para InputFields de quantidade e preço (definido na UI)
    public TMP_InputField inputQuantidadeMalha;
    public TMP_InputField inputQuantidadeElastico;
    public TMP_InputField inputQuantidadeEmbalagens;
    public TMP_InputField inputQuantidadeEtiquetas;
    public TMP_InputField inputQuantidadeLinha;
    public TMP_InputField inputQuantidadeVenda;
    public TMP_InputField inputPrecoUnitarioVenda;  
    public TMP_InputField inputProducao;
    
    public GameManager gm;
    public GameObject telaResumo;
    public TMP_Text textoResumo;

    public float valorVendaNoTurno;
    public float vendasRealizadas;

    public float totalVendasRealizadas;
    public float totalDinheiroRecebido;


    private float totalRecebidoNoTurno;

    private float quantidadeJogador;
    
    public IAMuitoFoda scriptIA;


    public bool PodeEfetuarVenda()
    {
        // Verifica se os campos de quantidade e preço estão preenchidos
        return !string.IsNullOrEmpty(inputQuantidadeVenda.text) && !string.IsNullOrEmpty(inputPrecoUnitarioVenda.text);
    }
    
    

    public void EfetuarVenda()
    {
        // Lógica de venda aqui
        float quantidadeVendida = int.Parse(inputQuantidadeVenda.text);
        float precoUnitario = float.Parse(inputPrecoUnitarioVenda.text);
        
        float estoqueAtual = gm.estoqueProdutos;
        float demandaAtual = turnManager.demandaAtual;
        
        
        // Limita a quantidade de venda ao menor valor entre estoque atual e demanda do mercado
        quantidadeVendida = Math.Min(quantidadeVendida, Math.Min(estoqueAtual, demandaAtual));
        
        if (gm.estoqueProdutos >= quantidadeVendida && quantidadeVendida <= turnManager.demandaAtual)
        {
            float valorTotalVenda = quantidadeVendida * precoUnitario;
            
            valorVendaNoTurno = valorTotalVenda;
            vendasRealizadas = quantidadeVendida;

            totalVendasRealizadas += quantidadeVendida;
            totalDinheiroRecebido += valorTotalVenda;

            gm.estoqueProdutos -= quantidadeVendida;
            gm.dinheiro += valorTotalVenda;
            gm.totalVendas += valorTotalVenda;
            gm.AtualizarEstoqueUI();
                
            ExibirResumo();
            //telaResumo.SetActive(true);
            //turnManager.AvancarRodada();
        }
        else
        {
            Debug.Log("Venda não realizada: Produtos insuficientes ou excedeu o limite de vendas.");
        }

        
        // Exemplo de processamento da venda
        Debug.Log($"Venda efetuada: {quantidadeVendida} unidades a R$ {precoUnitario} cada.");
    }
    
    
    
public void ProcessarVenda()
{
    // 1. Capturar os inputs do jogador
    float precoJogador = float.Parse(inputPrecoUnitarioVenda.text);
    int quantidadeJogador = int.Parse(inputQuantidadeVenda.text);

    // 2. Ajustar os inputs do jogador (limita pela demanda e estoque)
    quantidadeJogador = Mathf.Min(quantidadeJogador, (int)gm.estoqueProdutos, (int)turnManager.demandaAtual);

    // 3. IA ajusta seus inputs (preço e quantidade)
    scriptIA.ImitarVenda(precoJogador, quantidadeJogador, turnManager.demandaAtual);
    float precoIA = scriptIA.GetPrecoIA();
    int quantidadeIA = Mathf.Min(scriptIA.GetQuantidadeIA(), (int)scriptIA.estoqueIA, (int)turnManager.demandaAtual);

    // 4. Comparar preços para decidir quem vende primeiro
    float demandaRestante = turnManager.demandaAtual;

    if (precoJogador < precoIA)
    {
        // Jogador vende primeiro, respeitando a demanda
        demandaRestante = VenderProduto(precoJogador, quantidadeJogador, ref gm.estoqueProdutos, ref gm.dinheiro, ref gm.totalVendas, demandaRestante);
        valorVendaNoTurno = precoJogador;
        vendasRealizadas = quantidadeJogador;

        totalRecebidoNoTurno = precoJogador * quantidadeJogador;
        totalDinheiroRecebido += precoJogador * quantidadeJogador;
        totalVendasRealizadas += quantidadeJogador;
        
        gm.AtualizarEstoqueUI();

        // Após a venda do jogador, IA tenta vender o restante da demanda
        if (demandaRestante > 0)
        {
            demandaRestante = VenderProduto(precoIA, quantidadeIA, ref scriptIA.estoqueIA, ref scriptIA.valorTotalRecebidoIA, ref scriptIA.vendasTotaisIA, demandaRestante);

            scriptIA.finalVendasIA += quantidadeIA;
        }
    }
    else
    {
        // IA vende primeiro, respeitando a demanda
        demandaRestante = VenderProduto(precoIA, quantidadeIA, ref scriptIA.estoqueIA, ref scriptIA.valorTotalRecebidoIA, ref scriptIA.vendasTotaisIA, demandaRestante);
        scriptIA.finalVendasIA += quantidadeIA;
        
        // Após a venda da IA, o jogador tenta vender o restante da demanda
        if (demandaRestante > 0)
        {
            demandaRestante = VenderProduto(precoJogador, quantidadeJogador, ref gm.estoqueProdutos, ref gm.dinheiro, ref gm.totalVendas, demandaRestante);
            valorVendaNoTurno = precoJogador;
            vendasRealizadas = quantidadeJogador;


            totalRecebidoNoTurno = precoJogador * quantidadeJogador;
            totalDinheiroRecebido += precoJogador * quantidadeJogador;
            totalVendasRealizadas += quantidadeJogador;
            
            gm.AtualizarEstoqueUI();
        }
    }

    // 5. Fim do turno se a demanda foi atendida
    
        Debug.Log("Fim de Turno: Toda a demanda foi atendida.");
        // Chamar o método para finalizar o turno, se necessário
        ExibirResumo();
    
}


// Método para realizar a venda, respeitando a demanda
private float VenderProduto(float preco, int quantidade, ref float estoque, ref float dinheiro, ref float totalVendas, float demandaRestante)
{
    // Vender o mínimo entre a quantidade disponível e a demanda restante
    int quantidadeVendida = Mathf.Min(quantidade, (int)demandaRestante);

    // Atualiza o estoque e o dinheiro
    estoque -= quantidadeVendida;
    float valorVenda = quantidadeVendida * preco;
    dinheiro += valorVenda;
    totalVendas += valorVenda;

    // Atualiza a demanda restante
    demandaRestante -= quantidadeVendida;

    Debug.Log($"Vendeu {quantidadeVendida} unidades a {preco}, valor total: {valorVenda}. Demanda restante: {demandaRestante}");

    return demandaRestante;
}


    public void ExibirResumo()
    {
        textoResumo.text = $"Vendas realizadas: {vendasRealizadas}\n " +
                           $"Valor total recebido: {totalRecebidoNoTurno}\n" +
                           $"Produtos no estoque: {gm.estoqueProdutos}";
        telaResumo.SetActive(true);
    }
    public void ConfirmarResumo()
    {
        telaResumo.SetActive(false);
        
        turnManager.AvancarRodada();
    }

    public void ZerarInputs()
    {
        inputQuantidadeVenda.text = string.Empty;
        inputPrecoUnitarioVenda.text = string.Empty;
        inputQuantidadeMalha.text = string.Empty;
        inputQuantidadeElastico.text = string.Empty;
        inputQuantidadeEtiquetas.text = string.Empty;
        inputQuantidadeLinha.text = string.Empty;
        inputQuantidadeMalha.text = string.Empty;
        inputQuantidadeEmbalagens.text = string.Empty;
        inputProducao.text = string.Empty;
    }
}
