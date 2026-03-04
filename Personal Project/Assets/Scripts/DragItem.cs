using UnityEngine;
using UnityEngine.UI; // 🌟 추가: Image 컴포넌트를 제어하기 위해 필요합니다.
using UnityEngine.EventSystems;

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
        // 1. 드래그 시작 시점의 부모(현재 슬롯)를 기억합니다.
        parentAfterDrag = transform.parent;

        // 2. 드래그 중에는 아이템이 다른 칸들에 가려지지 않도록,
        // 캔버스(가장 최상위 부모)의 맨 밑으로 빼서 화면의 최상단에 그리게 만듭니다.
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1.0f;

        // 3. 🌟 수정됨: 원래 위치가 아니라, '최종 결정된 부모(기존 슬롯 or 새 슬롯)'의 자식으로 들어갑니다.
        transform.SetParent(parentAfterDrag);
        
        // 4. 슬롯의 정중앙(0, 0, 0)으로 위치를 쏙 맞춥니다.
        _rectTransform.localPosition = Vector3.zero;
    }
    // 🌟 추가: 이 아이템 UI가 어떤 데이터를 담고 있는지 연결할 빈칸을 만듭니다.
    public ItemData itemData; 

    void Start()
    {
        // 🌟 추가: 게임이 시작될 때, 연결된 데이터의 색상으로 내 모습을 바꿉니다!
        if (itemData != null)
        {
            GetComponent<Image>().color = itemData.itemColor;
        }
    }
    
        // 🌟 새로 추가하는 함수: 마우스 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        // 만약 마우스 오른쪽 버튼(left)을 클릭했다면?
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 이 아이템의 데이터가 연결되어 있고, 그 타입이 '무기(Weapon)'라면!
            if (itemData != null && itemData.type == ItemType.Weapon)
            {
                // BattleManager를 불러서 무기의 공격력(value)만큼 데미지를 줍니다!
                BattleManager.instance.TakeDamage(itemData.value);
            }
        }
    }

}