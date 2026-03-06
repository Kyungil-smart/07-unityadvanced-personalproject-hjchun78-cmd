using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // 🌟 추가됨: 씬 매니저(재시작) 부품!

public enum BattleState { PlayerTurn, EnemyTurn, Won, Lost }


public class BattleManager : MonoBehaviour
{
    [Header("Item Spawn System")]
    public GameObject baseItemPrefab; // 방금 만든 기본 아이템 프리팹
    public Transform itemSpawnPoint;  // 아이템이 나타날 위치
    public float slotSize = 100f;     // 🌟 인벤토리 1칸의 픽셀 크기 (그리드의 Cell Size와 똑같이 맞춰주세요!)
    public static BattleManager instance;
    public BattleState currentState;

    [Header("Enemy Stats (흑기사)")]
    public GameObject enemyObject;
    public int enemyHP = 100;
    public int enemyDamage = 10;
    public TextMeshProUGUI enemyHPText;

    private SpriteRenderer enemySprite;
    private Animator enemyAnimator;

    [Header("Player Stats")]
    public int playerMaxHP = 200;
    public int playerHP = 50;
    public int maxEnergy = 3;      // 🌟 한 턴에 주어지는 최대 행동력 (예: 3번 공격 가능)
    public int currentEnergy;      // 🌟 현재 남아있는 행동력
    public int playerBlock = 0;    // 🌟 추가됨: 현재 쌓인 방어력
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI energyText; // 🌟 아까 만든 EnergyText를 연결할 빈칸
    public TextMeshProUGUI blockText; // 🌟 추가됨: 방어력 텍스트 빈칸
    public SpriteRenderer playerSprite;
    public Animator playerAnimator;

    [Header("UI Panels")]
    public GameObject gameOverPanel; // 🌟 추가됨: 방금 만든 게임 오버 패널을 넣을 곳
    public GameObject victoryPanel; // 🌟 추가됨: 승리했을 때 띄울 패널

    void Awake()
    {
        instance = this;
        currentState = BattleState.PlayerTurn;
        currentEnergy = maxEnergy; // 🌟 게임 시작 시 행동력 가득 채우기!

        if (enemyObject != null)
        {
            enemySprite = enemyObject.GetComponentInChildren<SpriteRenderer>();
            enemyAnimator = enemyObject.GetComponentInChildren<Animator>();
        }

        else
        {
            currentState = BattleState.PlayerTurn;
            currentEnergy = maxEnergy; 
            playerBlock = 0; 

            SpawnRandomItem(); // 🌟 내 턴이 오면 새 아이템 소환!
        }

        UpdateUI();
        
    }

    // 1. 무기 공격 함수
    public bool TakeDamage(int damage)
    {
        if (currentState != BattleState.PlayerTurn) return false;
        if (currentEnergy <= 0)
        {
            Debug.Log("행동력이 부족합니다!");
            return false;
        }

        currentEnergy--; // 행동력 1 소모
        enemyHP -= damage;

        // 🌟 앗차차! 빠뜨렸던 타격 애니메이션과 이펙트를 복구합니다.
        if (playerAnimator != null) playerAnimator.SetTrigger("Attack1");
        if (enemySprite != null) StartCoroutine(HitFlash(enemySprite));
        if (enemyAnimator != null) enemyAnimator.SetTrigger("hit_1");

        // 🌟 적 체력이 0이 되었을 때의 승리 로직도 잊지 않고 챙겨줍니다!
        if (enemyHP <= 0)
        {
            enemyHP = 0;
            currentState = BattleState.Won;
            if (enemyAnimator != null) enemyAnimator.SetTrigger("death");
            Debug.Log("🎉 흑기사를 물리쳤습니다! 게임 승리!");

            StartCoroutine(ShowVictoryRoutine());
        }
        
        UpdateUI();
        return true; // 공격 성공!
    }
    // 2. 방패 사용 함수
    public bool UseShield(int blockValue)
    {
        if (currentState != BattleState.PlayerTurn) return false;
        if (currentEnergy <= 0)
        {
            Debug.Log("행동력이 부족합니다!");
            return false;
        }

        currentEnergy--;
        playerBlock += blockValue; 
        if (playerAnimator != null) playerAnimator.SetTrigger("Block");

        UpdateUI();
        return true; // 🌟 방어 성공!
    }

    // 3. 포션 사용 함수 (🌟 GameObject 받는 부분 삭제됨!)
    public bool UsePotion(int healAmount)
    {
        if (currentState != BattleState.PlayerTurn) return false;
        if (currentEnergy <= 0)
        {
            Debug.Log("행동력이 부족합니다!");
            return false;
        }

        currentEnergy--;
        playerHP += healAmount;
        if (playerHP > playerMaxHP) playerHP = playerMaxHP; 

        if (playerSprite != null) StartCoroutine(HealFlash(playerSprite));
        
        UpdateUI();
        return true; // 🌟 회복 성공!
    }

    void UpdateUI()
    {
        if (enemyHPText != null) enemyHPText.text = "Enemy HP: " + enemyHP;
        if (playerHPText != null) playerHPText.text = "Player HP: " + playerHP;
        if (energyText != null) energyText.text = "Energy: " + currentEnergy + " / " + maxEnergy;
        
        // 🌟 방어력이 0보다 클 때만 화면에 표시해줍니다.
        if (blockText != null) 
        {
            if (playerBlock > 0) blockText.text = "Block: " + playerBlock;
            else blockText.text = ""; // 0이면 글씨 숨기기
        }
    }


    public void EndPlayerTurn()
    {
        if (currentState == BattleState.PlayerTurn)
        {
            currentState = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    // 🌟 턴을 넘기면 적이 공격하는 로직
    IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        if (enemyAnimator != null) enemyAnimator.SetTrigger("skill_1");
        yield return new WaitForSeconds(0.5f);

        // 🌟 방어력 계산 로직 시작
        int damageTaken = enemyDamage;

        if (playerBlock > 0)
        {
            if (playerBlock >= damageTaken) // 방어력이 데미지보다 높을 때
            {
                playerBlock -= damageTaken;
                damageTaken = 0; // 데미지 완벽 방어!
                if (playerAnimator != null) playerAnimator.SetTrigger("Block"); // 막는 모션 재생
            }
            else // 방어력이 깨졌을 때
            {
                damageTaken -= playerBlock;
                playerBlock = 0;
            }
        }

        playerHP -= damageTaken; // 계산된 최종 데미지만큼만 체력 감소

        // 데미지를 1이라도 입었을 때만 아파하는 모션(Hurt) 재생
        if (damageTaken > 0)
        {
            if (playerSprite != null) StartCoroutine(HitFlash(playerSprite));
            if (playerAnimator != null) playerAnimator.SetTrigger("Hurt");
        }

        if (playerHP <= 0)
        {
            playerHP = 0;
            currentState = BattleState.Lost;
            if (playerAnimator != null) playerAnimator.SetTrigger("Death");
            Debug.Log("💀 패배했습니다...");

            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }
        else
        {
            currentState = BattleState.PlayerTurn;
            currentEnergy = maxEnergy; 
            playerBlock = 0; // 🌟 턴이 새로 시작되면 남은 방어력은 사라집니다 (초기화)
            SpawnRandomItem(); // 🌟 적 턴이 무사히 끝나면 새 아이템 1개 소환!
        }
        UpdateUI();
    }

    public void RestartGame()
    {
        // 현재 열려있는 씬의 이름을 알아내서 아예 처음부터 다시 불러옵니다!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GoToTitle()
    {
        // 괄호 안에는 Build Settings에 등록해둔 타이틀 씬 이름을 정확히 적습니다.
        SceneManager.LoadScene("1. Title");
    }

    IEnumerator HitFlash(SpriteRenderer targetSprite)
    {
        if (targetSprite != null)
        {
            Color originalColor = Color.white;
            targetSprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            targetSprite.color = originalColor;
        }
    }

    // 🌟 추가됨: 초록색 회복 이펙트
    IEnumerator HealFlash(SpriteRenderer targetSprite)
    {
        if (targetSprite != null)
        {
            Color originalColor = Color.white;
            targetSprite.color = Color.green; // 초록색으로 변함
            yield return new WaitForSeconds(0.1f);
            targetSprite.color = originalColor;
        }
    }

    // 🌟 랜덤 아이템을 소환하는 함수
    public void SpawnRandomItem()
    {
        if (baseItemPrefab == null || itemSpawnPoint == null) return;

        // 1. 프리팹을 소환진에 생성합니다.
        GameObject go = Instantiate(baseItemPrefab, itemSpawnPoint);
        DragItem dragItem = go.GetComponent<DragItem>();

        // 2. 메모리 상에서 즉석으로 새로운 아이템 데이터를 하나 찍어냅니다!
        ItemData newData = ScriptableObject.CreateInstance<ItemData>();

        // 3. 종류와 색상 랜덤 결정 (0: 무기, 1: 방패, 2: 포션)
        int typeRandom = Random.Range(0, 3);
        if (typeRandom == 0)
        {
            newData.type = ItemType.Weapon;
            newData.itemName = "랜덤 무기";
            newData.itemColor = Color.red;
        }
        else if (typeRandom == 1)
        {
            newData.type = ItemType.Shield;
            newData.itemName = "랜덤 방패";
            newData.itemColor = Color.blue;
        }
        else
        {
            newData.type = ItemType.Potion;
            newData.itemName = "랜덤 포션";
            newData.itemColor = Color.green;
        }

        // 4. 랜덤 크기 결정 (1~2칸)
        newData.width = Random.Range(1, 3);
        newData.height = Random.Range(1, 3);

        // 5. 능력치 결정: 넓이(칸 수) * 5 (예: 1칸=5, 4칸=20)
        int area = newData.width * newData.height;
        newData.value = area * 5;

        // 6. 생성된 데이터 연결 및 색상 적용
        dragItem.itemData = newData;
        go.GetComponent<UnityEngine.UI.Image>().color = newData.itemColor;

        // 7. 칸 수에 맞게 UI 이미지 크기를 쫙 늘려줍니다!
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(newData.width * slotSize, newData.height * slotSize);
        
        Debug.Log($"{newData.itemName} ({newData.width}x{newData.height}) 생성! 능력치: {newData.value}");
    }
    // 🌟 승리 창을 띄우는 지연 코루틴 (이 부분이 빠져있었습니다!)
    IEnumerator ShowVictoryRoutine()
    {
        // 흑기사가 바닥에 쓰러질 때까지 2초 기다려줍니다.
        yield return new WaitForSeconds(2.0f); 
        
        // 숨겨뒀던 승리 창 짠!
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

}