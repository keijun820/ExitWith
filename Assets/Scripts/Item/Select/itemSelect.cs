using UnityEngine;
using UnityEngine.UI;

public class itemSelect : MonoBehaviour
{
    [SerializeField] GameObject[] BG; //0 now 1 next 2 prev
    [SerializeField] Image[] itemSlotImage;
    [SerializeField] Sprite[] itemImages;
    [SerializeField] bool[] itemList;
    int marker;

    int itemNumber = 9;

    void Start()
    {
        marker = 0;
        showItems();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) showItems();

        float scroll = Input.mouseScrollDelta.y;
        if (scroll == 0f) return;

        marker += scroll > 0f ? 1 : -1;
        while(marker >= itemNumber) marker -= itemNumber;
        while(marker < 0) marker += itemNumber;
        showItems();
    }

    void showItems()
    {
        int now = -1;
        int next = -1;
        int prev = -1;
        for(int i = 0; i < itemNumber; i++)
        {
            int index = marker + i;
            if (index >= itemNumber) index -= itemNumber;

            if (itemList[index])
            {
                if(next != -1) prev = index;
                if (now != -1 && next == -1) next = index;
                else if(now == -1) now = index;
            }
        }
        for (int i = 0; i < 3; i++) BG[i].SetActive(false);
        if (now == -1) return;
        else
        {
            BG[0].SetActive(true);
            itemSlotImage[0].sprite = itemImages[now];
            if(next != -1)
            {
                BG[1].SetActive(true);
                itemSlotImage[1].sprite = itemImages[next];
                if(prev != -1)
                {
                    BG[2].SetActive(true);
                    itemSlotImage[2].sprite = itemImages[prev];
                }
            }
        }
    }

    public void updateItemList()
    {

    }
}
