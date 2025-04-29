using UnityEngine;

public abstract class Cenario
{
    // Nome do cenário para exibição
    public string nomeCenario;

    // Construtor para definir o nome
    public Cenario(string nome)
    {
        nomeCenario = nome;
    }

    // Método abstrato que será sobrescrito nos cenários específicos
    public abstract void AplicarCenario(TurnManager turnManager);

    public abstract string GetDescricao();
}