using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    // 🌟 이 슬롯의 좌표를 기억할 변수 추가
    public int posX; 
    public int posY;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DragItem dragItem = dropped.GetComponent<DragItem>();

        if (dragItem != null)
        {
            // 1. 매니저에게 이 좌표(posX, posY)에 아이템 크기만큼 빈 공간이 있는지 물어봅니다!
            if (InventoryManager.instance.CheckAvailableSpace(posX, posY, dragItem.itemData.width, dragItem.itemData.height))
            {
                // 2. 공간이 넉넉하다면, 아이템이 돌아갈 진짜 부모를 이 슬롯으로 정해줍니다.
                dragItem.parentAfterDrag = transform;
                
                // 3. 아이템에게 자기가 가방 안의 몇 콤마 몇(X, Y)에 들어왔는지 알려줍니다.
                dragItem.currentX = posX;
                dragItem.currentY = posY;
                
                Debug.Log(dragItem.itemData.itemName + " 배치");
            }
            else
            {
                Debug.Log("공간이 부족하거나 다른 아이템이 가로막고 있습니다!");
                // (배치 실패 시 아이템은 알아서 원래 있던 자리로 돌아갑니다)
            }
        }
    }
}