using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    public List<CardData> deckCads = new List<CardData>();              //덱에 있는 카드
    public List<CardData> handCads = new List<CardData>();              //손에 잇는 카드
    public List<CardData> discardCads = new List<CardData>();           //버린 카드 더미

    public GameObject cardPrefabs;                                      //카드 프리팹
    public Transform deckPosition;                                      //덱 위치
    public Transform handPosition;                                      //손 중앙 위치
    public Transform discardPosition;                                   //버린 카드 더미 위치

    public List<GameObject> cardObjects = new List<GameObject>();       //실제 카드 게임 오브젝트들

    public CharacterState playerStats;

    private static CardManager instance;

    public static CardManager Instance
    {
        get
        {
            if(instance == null) instance=new CardManager();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ShuffleDect();                                  //시작시 카드 섞기
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))                //D키 누르면 카드 드로우
        {
            DrawCard();
        }

        if (Input.GetKeyDown(KeyCode.F))                //F키를 누르면 버린 카드를 덱으로 되돌리고 섞기
        {
            ReturnDiscardsToDeck();
        }

        ArrengeHand();                                  //손패 위치 업데이트
    }


    public void ShuffleDect()       //덱 섞기
    {   
        List<CardData> tempDeck = new List<CardData>(deckCads);                 //임시 리스트에 카드 복사
        deckCads.Clear();

        while (tempDeck.Count > 0)                          //랜덤하게 섞기
        {
            int randIndex = Random.Range(0, tempDeck.Count);
            deckCads.Add(tempDeck[randIndex]);
            tempDeck.RemoveAt(randIndex);
        }

        Debug.Log("덱을 섞었습니다. : " + deckCads.Count + "장");

    }

    public void DrawCard()
    {
        if (handCads.Count >= 6)                //손패가 이미 6장 이상이면 드로우 하지 않음.
        {
            Debug.Log("손패가 가득 찼습니다. ! (최대 6장)");
            return;
        }

        if (deckCads.Count == 0)
        {
            Debug.Log("덱에 카드가 없습니다");
            return;
        }

        //덱에서 맨 위 카드 가져오기
        CardData cardData = deckCads[0];
        deckCads.RemoveAt(0);

        //손패에 추가
        handCads.Add(cardData);

        //카드 게임 오브젝트 생성
        GameObject cardObj = Instantiate(cardPrefabs,deckPosition.position, Quaternion.identity);

        //카드 정보 설정
        CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();

        if (cardDisplay != null)
        {
            cardDisplay.SetupCard(cardData);
            cardDisplay.cardIndex = handCads.Count - 1;
            cardObjects.Add(cardObj);
        }

        Debug.Log("카드를 드로우 했습니다 : "+ cardData.cardName + "손패 : "+handCads.Count + "/6");
        }

    public void ArrengeHand()
    {
        if (handCads.Count == 0) return;

        float cardWidth = 1.2f;
        float spacing = cardWidth + 1.8f;
        float totalWidth = (handCads.Count - 1) * spacing;
        float startX = -totalWidth / 2f;

        //각 카드 위치 설정
        for(int i =0; i< cardObjects.Count; i++)
        {
            if (cardObjects[i] != null)
            {
                CardDisplay display = cardObjects[i].GetComponent<CardDisplay>();

                if (display != null && display.isDragging) continue;
                
                    Vector3 targetPosition = handPosition.position + new Vector3(startX+(i*spacing),0,0);

                cardObjects[i].transform.position = Vector3.Lerp(cardObjects[i].transform.position, targetPosition, Time.deltaTime * 10f);
            }
            }

    }
    public void DiscardCard(int handIndex)
    {
        if (handIndex < 0 || handIndex >= handCads.Count)
        {
            Debug.Log("유효하지 않은 카드 인덱스 입니다");
            return;
        }

        CardData cardData = handCads[handIndex];                //손패에서 카드 가져오기
        handCads.RemoveAt(handIndex);

        discardCads.Add(cardData);                      //버린 카드 더미에 추가

        if(handIndex < cardObjects.Count)               //해당 카드 게임 오브젝트 제거
        {
            Destroy(cardObjects[handIndex]);
            cardObjects.RemoveAt(handIndex);
        }

        for(int i =0; i < cardObjects.Count; i++)       //카드 인덱스 재설정
        {
            CardDisplay display = cardObjects[i].GetComponent<CardDisplay>();
            if (display != null) display.cardIndex = i;
        }
        ArrengeHand();                                              //손패 위치 업데이트
        Debug.Log("카드를 버렸습니다 " + cardData.cardName);
    }

    //버린 카드를 덱으로 되돌리고 섞기
    public void ReturnDiscardsToDeck()
    {
        if(discardCads.Count == 0)
        {
            Debug.Log("버린 카드 더미가 비어 있습니다.");
            return;
        }

        deckCads.AddRange(discardCads);                         //버린 카드를 모두 덱에 추가
        discardCads.Clear();                                    //버린 카드 더미 비우기
        ShuffleDect();                                          //덱 섞기

        Debug.Log("버린 카드 " + deckCads.Count + "장을 덱으로 되돌리고 섞었습니다");

    }

}
