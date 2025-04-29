using UnityEngine;

public class CenarioManager
{
    private Cenario[] cenariosDisponiveis;
    private Cenario cenarioAtual;
    public CenarioManager()
    {
        // Inicializa os cenários disponíveis
        cenariosDisponiveis = new Cenario[] {/* new CenarioReduzTempo()*/ new CenarioAumentaPrecoMateriais() };
    }

    /*
    public void SortearCenario(TurnManager turnManager)
    {
        int indiceCenario = Random.Range(0, cenariosDisponiveis.Length);
        cenarioAtual = cenariosDisponiveis[indiceCenario];
        
        cenarioAtual.AplicarCenario(turnManager);

        //Debug.Log($"Cenário sorteado: {cenarioAtual.GetDescricao()}");
    }
*/
    public void SortearCenario(TurnManager turnManager)
    {
        int indiceCenario = Random.Range(0, cenariosDisponiveis.Length);
        cenarioAtual = cenariosDisponiveis[indiceCenario];
        // Apenas define o cenário atual, sem aplicá-lo diretamente
        Debug.Log($"Cenário sorteado: {cenarioAtual.GetDescricao()}");
    }

    public string ObterDescricaoCenario()
    {
        return cenarioAtual.GetDescricao();
    }

}