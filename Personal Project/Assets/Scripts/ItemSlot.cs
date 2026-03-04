using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // 🌟 핵심 추가: 내 칸 안에 자식 오브젝트(아이템)가 0개일 때만(비어있을 때만) 받아줍니다.
        if (transform.childCount == 0)
        {
            if (eventData.pointerDrag != null)
            {
                DragItem draggedItem = eventData.pointerDrag.GetComponent<DragItem>();

                if (draggedItem != null)
                {
                    // 목적지를 나(현재 슬롯)로 변경!
                    draggedItem.parentAfterDrag = transform;
                }
            }
        }
        // 자식 오브젝트가 1개 이상(이미 아이템이 있음)이라면 이 코드를 무시하게 되고,
        // 아이템은 원래 있던 자리로 고무줄처럼 튕겨 돌아가게 됩니다!
    }
}