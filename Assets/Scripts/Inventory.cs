using System.Collections.Generic;

public class Inventory
{
    // Dicionário para armazenar o estoque de cada tipo de matéria-prima
    private Dictionary<MaterialType, float> stock;

    public Inventory()
    {
        stock = new Dictionary<MaterialType, float>
        {
            { MaterialType.MalhaPVBranca, 0f },
            { MaterialType.Elastico, 0f },
            { MaterialType.Embalagens, 0f },
            { MaterialType.Etiquetas, 0f },
            { MaterialType.Linha, 0f }
        };
    }

    // Método para adicionar quantidade ao estoque
    public void AddStock(MaterialType type, float amount)
    {
        if (stock.ContainsKey(type))
        {
            stock[type] += amount;
        }
    }

    // Método para remover quantidade do estoque
    public bool RemoveStock(MaterialType type, float amount)
    {
        if (stock.ContainsKey(type) && stock[type] >= amount)
        {
            stock[type] -= amount;
            return true;
        }
        return false;
    }

    // Método para obter a quantidade atual de um tipo específico de matéria-prima
    public float GetStock(MaterialType type)
    {
        return stock.ContainsKey(type) ? stock[type] : 0f;
    }
}

// Enum para os tipos de matéria-prima
public enum MaterialType
{
    MalhaPVBranca,
    Elastico,
    Embalagens,
    Etiquetas,
    Linha
}