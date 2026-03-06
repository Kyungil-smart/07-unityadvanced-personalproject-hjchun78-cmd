using UnityEngine;
using UnityEngine.UI; // 🌟 추가: Image 컴포넌트를 제어하기 위해 필요합니다.
using UnityEngine.EventSystems;
using System.ComponentModel;

// 🌟 추가: 마우스 클릭을 감지하는 IPointerClickHandler 추가
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    // 🌟 추가됨: 다른 스크립트(ItemSlot)에서 이 값을 바꿀 수 있도록 public으로 엽니다.
    [HideInInspector] public Transform parentAfterDrag; 

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        
        // 🌟 수정됨: 에러 방지를 위해 게임 화면의 '진짜 캔버스'를 이름으로 직접 찾아갑니다!
        GameObject mainCanvas = GameObject.Find("Canvas");
        if (mainCanvas != null) transform.SetParent(mainCanvas.transform);
        transform.SetAsLastSibling();
        
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 1.0f; 

        if (currentX >= 0 && currentY >= 0)
        {
            InventoryManager.instance.PickUpItem(currentX, currentY, itemData.width, itemData.height);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1.0f;

        GameObject hitObj = eventData.pointerCurrentRaycast.gameObject;
        if (hitObj == null || hitObj.GetComponentInParent<ItemSlot>() == null)
        {
            Debug.Log("아이템을 가방 밖으로 버렸습니다!");
            Destroy(gameObject);
            return; 
        }

        transform.SetParent(parentAfterDrag);
        
        // 🌟 [핵심] 백팩 히어로처럼 딱 맞게 끼워 넣는 수학 공식!
        float slotSize = BattleManager.instance.slotSize;
        
        // 아이템이 여러 칸일 경우, 내려놓은 슬롯을 기준으로 우측 하단으로 살짝 밀어줍니다.
        float offsetX = (itemData.width - 1) * (slotSize / 2f);
        float offsetY = (itemData.height - 1) * -(slotSize / 2f); // 유니티 UI에서 아래쪽은 마이너스(-)
        
        // 계산된 위치로 정밀하게 쏙 끼워넣기!
        _rectTransform.localPosition = new Vector3(offsetX, offsetY, 0);

        if (currentX >= 0 && currentY >= 0)
        {
            InventoryManager.instance.PlaceItem(currentX, currentY, itemData.width, itemData.height);
        }
    }

    // 🌟 추가: 이 아이템 UI가 어떤 데이터를 담고 있는지 연결할 빈칸을 만듭니다.
    public ItemData itemData;    
    // 🌟 추가됨: 내가 현재 가방 안의 어느 좌표에 있는지 기억합니다. (처음엔 가방 밖이므로 -1)
    public int currentX = -1; 
    public int currentY = -1;
    public int durability = 5;

    void Start()
    {
        // 🌟 추가: 게임이 시작될 때, 연결된 데이터의 색상으로 내 모습을 바꿉니다!
        if (itemData != null)
        {
            GetComponent<Image>().color = itemData.itemColor;
        }
    }
    
    // 🌟 새로 추가하는 함수: 마우스 클릭 이벤트
    // 🌟 수정된 마우스 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (BattleManager.instance.currentState == BattleState.PlayerTurn) 
            {
                if (itemData != null)
                {
                    bool isUsed = false; // 방금 우클릭으로 아이템 쓰기에 성공했는지 확인

                    // 아이템 종류별로 매니저에게 사용 요청
                    if (itemData.type == ItemType.Weapon)
                    {
                        isUsed = BattleManager.instance.TakeDamage(itemData.value);
                    }
                    else if (itemData.type == ItemType.Shield)
                    {
                        isUsed = BattleManager.instance.UseShield(itemData.value);
                    }
                    else if (itemData.type == ItemType.Potion)
                    {
                        isUsed = BattleManager.instance.UsePotion(itemData.value);
                        if (isUsed) durability = 1; // 포션은 1번 쓰면 끝이므로 내구도를 1로 취급
                    }

                    // 🌟 행동력이 있어서 아이템을 성공적으로 썼다면?
                    if (isUsed)
                    {
                        durability--; // 내구도 1 깎기
                        Debug.Log(itemData.itemName + " 사용! 남은 내구도: " + durability);

                        // 내구도가 0이 되었다면 파괴!
                        if (durability <= 0)
                        {
                            Debug.Log(itemData.itemName + "이(가) 부서졌습니다!");
                            
                            // 1. 가방 엑셀 표에서 내 자리를 다시 '빈 칸(false)'으로 치워줍니다.
                            if (currentX >= 0 && currentY >= 0)
                            {
                                InventoryManager.instance.PickUpItem(currentX, currentY, itemData.width, itemData.height);
                            }
                            
                            // 2. 화면에서 아이템 완전히 삭제!
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }
}