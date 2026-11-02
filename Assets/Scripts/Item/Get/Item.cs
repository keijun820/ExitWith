using UnityEngine;

// 월드에 있는 모든 아이템 오브젝트에 공통으로 붙이는 스크립트.
// 습득 로직은 여기 없고, 카메라에 붙은 ItemInteractor가 레이캐스트로 이 스크립트를 찾아서 처리합니다.
// 이 스크립트는 아이템의 정보(이름, 열쇠 여부)만 가지고 있습니다.
// 콜라이더 필요 (레이캐스트가 맞아야 감지됨), Outline 컴포넌트가 있으면 윤곽선도 자동으로 켜짐.
[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
    public string itemName;    // 이 아이템의 이름 (인스펙터에서 오브젝트마다 다르게 설정)
    public bool isKey = false; // 열쇠라면 체크 (열쇠는 개별 관리되어 2개 이상이면 꾸러미로 표시됨)
}