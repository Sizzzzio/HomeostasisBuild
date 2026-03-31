[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public void Add(int amount)
    {
        quantity += amount;
    }
}