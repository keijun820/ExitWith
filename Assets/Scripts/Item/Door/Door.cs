using UnityEngine;

// 문에 부착하는 스크립트. 지정한 열쇠를 가지고 있어야만 열립니다.
// 콜라이더 필요 (레이캐스트가 맞아야 감지됨).
[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour
{
    public string requiredKeyName; // 이 문을 여는 데 필요한 열쇠 이름 (예: "창고 열쇠")
    public bool isOpen = false;

    // 인벤토리에 맞는 열쇠가 있으면 문을 열고 true, 없으면 false 반환
    public bool TryOpen(SimpleInventory inventory)
    {
        if (isOpen)
        {
            Debug.Log(gameObject.name + " 은(는) 이미 열려있습니다.");
            return true;
        }

        if (inventory.HasKey(requiredKeyName))
        {
            isOpen = true;
            Debug.Log(gameObject.name + " 문이 열렸습니다!");
            // TODO: 실제 문 여는 연출은 여기에 추가 (애니메이션 재생, 콜라이더 끄기, 회전시키기 등)
            return true;
        }
        else
        {
            Debug.Log(requiredKeyName + " 이(가) 없습니다.");
            return false;
        }
    }
}