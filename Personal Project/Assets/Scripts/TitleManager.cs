using UnityEngine;
using UnityEngine.SceneManagement; // 🌟 씬 이동을 위해 반드시 필요!

public class TitleManager : MonoBehaviour
{
    // Start 버튼을 누르면 실행될 함수
    public void GameStart()
    {
        // 괄호 안에는 아까 Build Settings에 넣은 씬 이름을 정확히 적어줍니다.
        SceneManager.LoadScene("2. GamePlay"); 
    }

    // Quit 버튼을 누르면 실행될 함수
    public void GameQuit()
    {
        Debug.Log("게임 종료!"); // 유니티 에디터에서 확인용
        Application.Quit(); // 실제 빌드된 게임을 꺼버리는 마법의 코드
    }
}