using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AllCardCon : MonoBehaviour
{
    public static int mId;
    public Transform[] players;
    public Transform table;

    public float cardDisplayWidth;

    private Transform[] cardsTrans;
    private CardControl[] cardCons;

    public static int dragingCard = -1;

    public GameObject askUI;
    public Text askUIText;

    public GameObject orderUI;
    public bool clockwise;


    public static AllCardCon allCardCon;

    private int flying;
    private int givingPlayer = 0;
    private int givingCard = 0;
    //private int[] order;

    void Awake()
    {
        allCardCon = this;
        int children = transform.childCount;
        cardsTrans = new Transform[children];
        cardCons = new CardControl[children];
        for (int i = 0; i < children; ++i)
        {
            cardsTrans[i] = transform.GetChild(i);
            cardCons[i] = cardsTrans[i].gameObject.AddComponent<CardControl>();
            cardCons[i].cardId = i;
        }
        
    }

    void Start() {
        mId = 0;
        askUI.SetActive(false);
        GameObject.Find("ani_Text_Button").GetComponent<Text>().text = (Data.NeedAnimation) ? "發牌過程on" : "發牌過程off";
    }

    public void StartDistribute(int player) {
        if (Data.NeedAnimation)
        {
            flying = Data.PlayerCardNumber * Data.PlayerNumber;
            Invoke("distribute", 0.3f);
        }
        else
        {
            flying = 0;
            for (int i = 0; i < Data.PlayerNumber; i++)
            {
                for (int j = 0; j < Data.PlayerCardNumber; j++)
                {
                    Give(i, new int[] { Data.Cards[j + i * Data.PlayerCardNumber] });
                    Debug.Log("person:" + i + " " + Data.Cards[j + i * Data.PlayerCardNumber]);
                }
            }
        }
    }

    void distribute()
    {
        int card = Data.Cards[givingCard];
        cardsTrans[card].SetParent(players[givingPlayer]);
        cardCons[card].ani = true;
        givingCard += 1;
        givingPlayer = (++givingPlayer >= Data.PlayerNumber) ? 0 : givingPlayer;

        if (givingCard < Data.PlayerNumber * Data.PlayerCardNumber)
            Invoke("distribute", 0.3f);
    }

    void Update()
    {
        if (flying > 0 || Data.waiting)
            return;

        if (mId == nextPlayerCon.player)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (string.Equals(hit.transform.tag, "Card"))
                    {
                        dragingCard = hit.collider.gameObject.GetComponent<CardControl>().cardId;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (dragingCard > -1)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        string hitTag = hit.transform.tag;
                        //click the card(take it)
                        if (string.Equals(hitTag, "Card") && Data.NeedDrawCard == true)
                        {
                            int oldParent = cardCons[dragingCard].getParent();
                            if (hit.collider.GetComponent<CardControl>().cardId == dragingCard)
                            {
                                Give(0, new int[] { dragingCard });
                                CardPosition(oldParent);
                            }
                        }
                        //give card away
                        if (string.Equals(hitTag, "Player"))
                        {
                            int oldParent = cardCons[dragingCard].getParent();
                            Give(hit.collider.GetComponent<OtherPlayer>().playerId, new int[] { dragingCard });
                            CardPosition(oldParent);
                        }
                        //put on table
                        if (string.Equals(hitTag, "Table"))
                        {
                            int oldParent = cardCons[dragingCard].getParent();
                            PutOnTable(hit.point, dragingCard);
                            CardPosition(oldParent);
                        }
                    }
                    dragingCard = -1;
                }
            }
        }
    }

    public void Give(int player, int[] cards)
    {
        foreach (int i in cards)
        {
            cardsTrans[i].SetParent(players[player]);
            cardsTrans[i].localRotation = Quaternion.Euler(Vector3.zero);
        }
        CardPosition(player);
    }

    public void Give(int player, int card)
    {
        cardsTrans[card].SetParent(players[player]);
        cardsTrans[card].localRotation = Quaternion.Euler(Vector3.zero);
        CardPosition(player);

        flying -= 1;
    }

    public void CardPosition(int player)
    {
        if (player > 3)
            return;

        Transform playerTrans = players[player];
        int children = playerTrans.childCount;
        if (flying > 0)
        {
            int i;
            for (i = 0; i < children; ++i)
                if (playerTrans.GetChild(i).GetComponent<CardControl>().ani == true)
                    break;
            children = i;
        }

        playerTrans = players[player];
        children = playerTrans.childCount;

        float gap = cardDisplayWidth / (children + 1);
        float xPos = -cardDisplayWidth / 2 + gap;
        for (int i = 0; i < children; ++i)
        {
            playerTrans.GetChild(i).localPosition = new Vector3(xPos, 0, i * -0.000001f);
            xPos += gap;
        }
    }

    public void PutOnTable(Vector3 point, int cardId) {
        Transform card = cardsTrans[cardId];
        card.SetParent(table);
        card.localPosition = new Vector3(point.x, 0, point.z);
        card.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));    
    }

    public void Ask(int player){
        askUIText.text = "玩家" + player.ToString() + "正要取走你一張牌";
        askUI.SetActive(true);
    }

    public void AskOK()
    {
        askUI.SetActive(false);
    }

    public void AskCancel() {
        askUI.SetActive(false);
    }

    public void aniBtnClick(Text text) {
        if (mId == 0)
        {
            Data.NeedAnimation = !Data.NeedAnimation;
            text.text = (Data.NeedAnimation) ? "發牌過程on" : "發牌過程off";
        }
    }

    public void reStart() {
        if(mId == 0)
            SceneManager.LoadScene(2);
    }

    public void exit() {
        SceneManager.LoadScene(0);
    }

    public void setClockwise(int clockwise) {
        switch (clockwise)
        {
            case 1:
                this.clockwise = true;
                break;
            case 0:
                this.clockwise = false;
                break;
            default:
                int i = Random.Range(0, 2);
                this.clockwise = i > 0;
                break;
        }
        orderUI.SetActive(false);
    }
}