using UnityEngine;

public class CenarioReduzTempo : Cenario
{
    private int tempoReducao;
    public CenarioReduzTempo() : base("Redução de Tempo")
    {
        tempoReducao = 120; // Reduz 2 minutos
    }

    public override void AplicarCenario(TurnManager turnManager)
    {
        // Aplica a redução de tempo se o tempo atual for igual ao original
        if (turnManager.tempoPorTurno == turnManager.tempoOriginal)
        {
            turnManager.turnTimer.tempoPorTurno -= tempoReducao; // Reduz o tempo
            //Debug.Log($"Cenário aplicado: {nomeCenario}. Tempo do turno reduzido em {tempoReducao} segundos.");
        }
    }

    public override string GetDescricao()
    {
        return "O tempo deste turno foi reduzido em 2 minutos.";
    }
}