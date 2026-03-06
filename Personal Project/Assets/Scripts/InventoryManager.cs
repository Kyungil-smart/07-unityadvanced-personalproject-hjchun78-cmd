using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // 어디서든 쉽게 부를 수 있게 싱글톤으로 만듭니다.
    public static InventoryManager instance;

    [Header("가방 크기 설정")]
    public int gridWidth = 3;  // 가로 칸 수
    public int gridHeight = 4; // 세로 칸 수

    // 🌟 핵심: 슬롯이 비어있는지 꽉 찼는지 기억하는 2차원 배열 (true면 꽉 참, false면 빈칸)
    public bool[,] gridStatus;

    void Awake()
    {
        instance = this;
        // 설정한 크기만큼의 빈 엑셀 표(배열)를 생성합니다.
        gridStatus = new bool[gridWidth, gridHeight];
    }

    // 특정 위치(startX, startY)에 아이템(너비, 높이)을 놓을 수 있는지 검사하는 함수
    public bool CheckAvailableSpace(int startX, int startY, int itemWidth, int itemHeight)
    {
        // 1. 아이템이 가방 범위를 벗어나는지 먼저 검사!
        if (startX + itemWidth > gridWidth || startY + itemHeight > gridHeight)
        {
            return false; // 가방 밖으로 삐져나감 (배치 불가)
        }

        // 2. 놓으려는 자리 중에 단 한 칸이라도 이미 다른 아이템이 차지하고 있는지 검사!
        for (int x = startX; x < startX + itemWidth; x++)
        {
            for (int y = startY; y < startY + itemHeight; y++)
            {
                if (gridStatus[x, y] == true) 
                {
                    return false; // 누군가 이미 자리를 차지함 (배치 불가)
                }
            }
        }

        return true; // 모든 검사 통과! (배치 가능)
    }

    // 아이템을 내려놓았을 때, 해당 칸들을 '사용 중(true)'으로 색칠하는 함수
    public void PlaceItem(int startX, int startY, int itemWidth, int itemHeight)
    {
        for (int x = startX; x < startX + itemWidth; x++)
        {
            for (int y = startY; y < startY + itemHeight; y++)
            {
                gridStatus[x, y] = true;
            }
        }
    }

    // 아이템을 집어 들었을 때, 해당 칸들을 다시 '빈 칸(false)'으로 지워주는 함수
    public void PickUpItem(int startX, int startY, int itemWidth, int itemHeight)
    {
        for (int x = startX; x < startX + itemWidth; x++)
        {
            for (int y = startY; y < startY + itemHeight; y++)
            {
                gridStatus[x, y] = false;
            }
        }
    }
}