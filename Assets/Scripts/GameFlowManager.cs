using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private GameObject menu; // Objeto do Menu
    [SerializeField] private GameObject gamePanels; // Objeto Paineis
    [SerializeField] private GameObject timer; // Objeto Timer

    private void Start()
    {
        // Exibe o menu inicialmente
        menu.SetActive(true);
        gamePanels.SetActive(false);
        timer.SetActive(false);
    }

    public void StartGame()
    {
        // Esconde o menu e exibe o jogo
        menu.SetActive(false);
        gamePanels.SetActive(true);
        timer.SetActive(true);

        Debug.Log("Jogo iniciado!");
    }
}