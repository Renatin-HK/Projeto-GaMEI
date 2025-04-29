using UnityEngine;

public class CenarioAumentaPrecoMateriais : Cenario
{
    public float aumentoPercentual;

    public CenarioAumentaPrecoMateriais() : base("Aumento de Preços de Materiais")
    {
        aumentoPercentual = 0.20f; // Aumenta em 20% o preço dos materiais
    }

    
    public override void AplicarCenario(TurnManager turnManager)
    {
        // Supondo que há um método para alterar o preço dos materiais no GameManager
        //turnManager.gm.AlterarPrecoMateriais(aumentoPercentual);
        //Debug.Log($"Cenário aplicado: {nomeCenario}. Preço dos materiais aumentado em {aumentoPercentual * 100}%.");
    }

    public override string GetDescricao()
    {
        return "Neste turno, o preço das matérias primas teve um aumento de 20%";
    }
}