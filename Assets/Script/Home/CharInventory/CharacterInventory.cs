using UnityEngine;
using System.Collections.Generic;

public class CharacterInventory : MonoBehaviour
{
    public static CharacterInventory Instance;

    // Tất cả nhân vật mà người chơi đã sở hữu
    public List<Character> ownedCharacters = new List<Character>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Lấy danh sách tướng đang được xếp vào đội hình
    public List<Character> GetLineupCharacters()
    {
        return ownedCharacters.FindAll(c => c.isLineup);
    }
}
