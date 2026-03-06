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

    // 🌟 새로 추가하는 부분: 아이템이 차지하는 가로, 세로 칸 수 (기본값은 1칸)
    public int width = 1;  
    public int height = 1;
    public Color itemColor = Color.white;
}
