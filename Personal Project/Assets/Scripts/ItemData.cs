using UnityEngine;

public enum ItemType 
{ 
    Weapon, 
    Potion, 
    Shield 
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;      // 인스펙터에 보이는 'Item Name'
    public ItemType type;        // 이게 있어야 'Type'이 보입니다!
    public int value;            // 이게 있어야 'Value'가 보입니다!
    public Color itemColor;      // 이게 있어야 'Item Color'가 보입니다!
}
