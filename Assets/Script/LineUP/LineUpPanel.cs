using System.Collections.Generic;
using UnityEngine;

public class LineupPanel : MonoBehaviour
{
    public List<LineUpSlotUI> slots = new List<LineUpSlotUI>();
    // thứ tự: Front1, Front2, Front3, Back1, Back2, Back3

    public void UpdateAllSlots()
    {
        // Cập nhật tất cả slot dựa trên dữ liệu từ Lineup.Instance.myLineup
        // tìm slot.position = lineup.Instance.myLineup.position để lấy characterID
        // sau đó lấy character từ CharacterInventory.Instance.ownedCharacters[characterID]
        //nếu <0 thì ClearSlot, ngược lại thì UpdateCard(character)
        foreach (var slot in slots)
        {
            Position pos = slot.position;
            PositionInLineup lineupPos = Lineup.Instance.myLineup.Find(p => p.position == pos);
            if (lineupPos == null || lineupPos.CharIndex < 0 || lineupPos.CharIndex >= CharacterInventory.Instance.ownedCharacters.Count)
            {
                slot.ClearSlot();
                continue;
            }
            Character character = CharacterInventory.Instance.ownedCharacters[lineupPos.CharIndex];
            slot.UpdateCard(character);
        }
    }
}