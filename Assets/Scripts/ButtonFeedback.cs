using UnityEngine;
using TMPro;

public class ButtonFeedback : MonoBehaviour
{
    public TMP_Text feedbackText; // Texto para mostrar sucesso ou falha
    public GameObject button; // O botão que será animado
    public Color successColor = Color.green; // Cor de sucesso
    public Color failureColor = Color.red; // Cor de falha
    public float feedbackDuration = 0.5f; // Duração do feedback

    // Método para disparar feedback de sucesso
    public void ShowSuccessFeedback()
    {
        feedbackText.text = "Compra bem-sucedida!";
        feedbackText.color = successColor;
        AnimateButton(true);
        ShowFeedbackText();
    }

    // Método para disparar feedback de falha
    public void ShowFailureFeedback()
    {
        feedbackText.text = "Falha ao comprar!";
        feedbackText.color = failureColor;
        AnimateButton(false);
        ShowFeedbackText();
    }

    // Método para animar o botão
    private void AnimateButton(bool isSuccess)
    {
        Color targetColor = isSuccess ? successColor : failureColor;

        LeanTween.scale(button, new Vector3(1.1f, 1.1f, 1), 0.1f)
            .setOnComplete(() => {
                LeanTween.scale(button, new Vector3(1f, 1f, 1), 0.1f);
                LeanTween.color(button, targetColor, feedbackDuration)
                    .setLoopPingPong();
            });
    }

    // Método para mostrar e esconder o texto de feedback
    private void ShowFeedbackText()
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.transform.localScale = new Vector3(0.5f, 0.5f, 1); // Começa pequeno
        LeanTween.scale(feedbackText.gameObject, new Vector3(1, 1, 1), 0.2f)
            .setOnComplete(() => {
                LeanTween.alpha(feedbackText.rectTransform, 0, feedbackDuration).setOnComplete(() => {
                    feedbackText.gameObject.SetActive(false);
                });
            });
    }
}