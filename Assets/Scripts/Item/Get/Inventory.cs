using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 아주 단순한 인벤토리: 일반 아이템은 이름 리스트로, 열쇠는 별도 리스트로 관리합니다.
// 이 스크립트 하나만 플레이어 오브젝트에 붙이면 끝입니다.
public class SimpleInventory : MonoBehaviour
{
    // ---- 일반 아이템 ----
    // 지금 가지고 있는 일반 아이템 이름들. 인스펙터에서도 눈으로 확인 가능.
    // 슬롯 수를 미리 정해두지 않고, 아이템을 습득할 때마다 리스트에 추가되면서 슬롯이 그만큼 늘어남.
    public List<string> items = new List<string>();

    // ---- 열쇠 ----
    // 지금까지 습득한 "서로 다른 열쇠"들의 이름 목록. 각 열쇠는 용도가 다르므로 개별 관리.
    // (예: "창고 열쇠", "서재 열쇠" 등)
    public List<string> keys = new List<string>();

    // 화면에 표시할 때 쓸 이름: 열쇠가 0개면 없음, 1개면 그 열쇠 이름 그대로,
    // 2개 이상이면 "열쇠 꾸러미"로 뭉뚱그려 보여줌. 내부적으로는 keys 리스트에 각 열쇠가 그대로 남아있음.
    public string KeyDisplayName
    {
        get
        {
            if (keys.Count == 0) return null;
            if (keys.Count == 1) return keys[0];
            return $"열쇠 꾸러미 ({keys.Count}개)";
        }
    }

    // 저장 파일이 어디 생길지 (신경 안 써도 되지만, 궁금하면 콘솔에서 확인 가능)
    string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    // ---- 일반 아이템 습득 ----
    // 습득하면 슬롯(items 리스트 칸)이 그만큼 늘어남. 성공하면 true, 이름이 비어있으면 false.
    public bool AddItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return false;

        items.Add(itemName); // 슬롯이 하나 늘어남
        Debug.Log($"{itemName} 획득! 현재 슬롯 수: {items.Count}");
        return true;
    }

    // ---- 일반 아이템 사용 ----
    // 사용한 아이템은 리스트에서 제거되어 슬롯 자체가 사라짐 (빈 칸이 남지 않음).
    // 가지고 있어서 사용에 성공하면 true, 가지고 있지 않으면 false.
    public bool UseItem(string itemName)
    {
        bool removed = items.Remove(itemName); // 리스트에서 하나 제거 → 슬롯이 그만큼 줄어듦
        if (removed)
            Debug.Log($"{itemName} 사용! 현재 슬롯 수: {items.Count}");
        else
            Debug.Log($"{itemName} 을(를) 가지고 있지 않습니다.");
        return removed;
    }

    // ---- 열쇠 습득 ----
    // 새로운 열쇠를 얻었으면 true, 이미 가지고 있는 열쇠라면(중복) false 반환
    public bool AddKey(string keyName)
    {
        if (string.IsNullOrEmpty(keyName)) return false;

        if (keys.Contains(keyName))
        {
            Debug.Log($"{keyName} 은(는) 이미 가지고 있습니다.");
            return false;
        }

        keys.Add(keyName);
        Debug.Log($"{keyName} 획득! (보유 열쇠: {KeyDisplayName})");
        return true;
    }

    // 특정 열쇠를 가지고 있는지 확인 (예: 문 열 때 사용)
    public bool HasKey(string keyName) => keys.Contains(keyName);

    // ---- 열쇠 사용 ----
    // 열쇠를 사용(소모)하면 keys 리스트에서 제거됨.
    // 그 결과 남은 개수가 2개 → 1개가 되면 KeyDisplayName이 자동으로 그 열쇠 이름으로 바뀌고,
    // 1개 → 0개가 되면 KeyDisplayName은 null이 되어 슬롯 자체가 사라진 것처럼 취급하면 됩니다.
    public bool UseKey(string keyName)
    {
        bool removed = keys.Remove(keyName);
        if (removed)
            Debug.Log($"{keyName} 사용! (보유 열쇠: {KeyDisplayName ?? "없음"})");
        else
            Debug.Log($"{keyName} 을(를) 가지고 있지 않습니다.");
        return removed;
    }

    // ---- 저장 ----
    // 원하는 타이밍(예: 저장 버튼, 씬 전환 전)에 이 함수만 호출하면 됩니다.
    public void Save()
    {
        Wrapper wrapper = new Wrapper { items = items, keys = keys };
        string json = JsonUtility.ToJson(wrapper);
        File.WriteAllText(SavePath, json);
        Debug.Log("저장 완료: " + SavePath);
    }

    // ---- 불러오기 ----
    // 게임 시작 시(Start 등)에 호출하면 이전 저장 상태를 복원합니다.
    public void Load()
    {
        if (!File.Exists(SavePath)) return; // 저장 파일이 없으면 그냥 새로 시작

        string json = File.ReadAllText(SavePath);
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
        items = wrapper.items ?? new List<string>();
        keys = wrapper.keys ?? new List<string>();
        Debug.Log("불러오기 완료");
    }

    // JsonUtility는 List를 최상위로 바로 못 바꿔서, 감싸주는 용도의 작은 클래스
    [System.Serializable]
    private class Wrapper
    {
        public List<string> items;
        public List<string> keys;
    }
}