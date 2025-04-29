using UnityEngine;
using System;

public class TurnTimer
{
    public float tempoRestante;
    public float tempoPorTurno;
    private Action<float, float> atualizarUITempoCallback;

    private Action onTempoEsgotadoCallback;

    public TurnTimer(float tempoInicial, Action<float, float> atualizarUITempo, Action onTempoEsgotado)
    {
        tempoPorTurno = tempoInicial;
        atualizarUITempoCallback = atualizarUITempo;
        onTempoEsgotadoCallback = onTempoEsgotado;
        ResetarTempo();
    }


    public void AtualizarTempo()
    {
        if (tempoRestante > 0)
        {
            tempoRestante -= Time.deltaTime;
            AtualizarUITempo();

            if (tempoRestante <= 0)
            {
                onTempoEsgotadoCallback?.Invoke(); // Chama o callback se o tempo acabar
            }
        }
    }


    public void ResetarTempo()
    {
        tempoRestante = tempoPorTurno;
        AtualizarUITempo();
    }
    
    public bool TempoEsgotado()
    {
        return tempoRestante <= 0;
    }

    private void AtualizarUITempo()
    {
        int minutos = Mathf.FloorToInt(tempoRestante / 60F);
        int segundos = Mathf.FloorToInt(tempoRestante % 60F);
        atualizarUITempoCallback(minutos, segundos);
    }
}