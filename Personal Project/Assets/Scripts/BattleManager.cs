using UnityEngine;
using TMPro; // TextMeshPro를 제어하기 위해 꼭 필요합니다.

public class BattleManager : MonoBehaviour
{
    // 🌟 핵심: 다른 스크립트(아이템)에서 이 매니저를 쉽게 부를 수 있도록 '싱글톤(Singleton)'으로 만듭니다.
    public static BattleManager instance; 

    public int enemyHP = 50; // 적의 기본 체력
    public TextMeshProUGUI enemyHPText; // 화면에 보여줄 텍스트 UI

    void Awake()
    {
        // 게임이 시작될 때 나 자신을 등록합니다.
        instance = this; 
        UpdateUI();
    }

    // 아이템이 적을 공격할 때 호출될 함수입니다.
    public void TakeDamage(int damage)
    {
        enemyHP -= damage;
        
        // 체력이 0 밑으로 떨어지지 않게 막아줍니다.
        if (enemyHP <= 0) 
        {
            enemyHP = 0;
            Debug.Log("적을 물리쳤습니다!");
        }

        UpdateUI();
        Debug.Log("적에게 " + damage + "의 피해를 입혔습니다!");
    }

    // 체력이 깎일 때마다 화면의 글씨를 갱신합니다.
    void UpdateUI()
    {
        if (enemyHPText != null)
        {
            enemyHPText.text = "Enemy HP: " + enemyHP;
        }
    }
}
