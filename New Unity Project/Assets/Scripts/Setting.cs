using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    Text player2;
    Text player3;
    Text player4;
    Text game_name;
    Text player_number;
    Text player_card_number;
    Text table_card_number;
    Toggle need_animation;
    Toggle need_draw_card;
  
    public void Start()
    {
        player2 = GameObject.Find("Canvas/conn/connect1/Text").GetComponent<Text>();
        player3 = GameObject.Find("Canvas/conn/connect2/Text").GetComponent<Text>();
        player4 = GameObject.Find("Canvas/conn/connect3/Text").GetComponent<Text>();
        game_name = GameObject.Find("Canvas/set/bg/game/Dropdown/Label").GetComponent<Text>();
        player_number = GameObject.Find("Canvas/set/bg/set/PlayerNumDropDown/Label").GetComponent<Text>();
        player_card_number = GameObject.Find("Canvas/set/bg/set/PlayerCardInputField/Text").GetComponent<Text>();
        table_card_number = GameObject.Find("Canvas/set/bg/set/TableCardInputField/Text").GetComponent<Text>();
        need_animation = GameObject.Find("Canvas/set/bg/Toggle").GetComponent<Toggle>();
        need_draw_card = GameObject.Find("Canvas/set/bg/Toggle2").GetComponent<Toggle>();
    }
    public void startBtn()
    {
        Data.Player2 = player2.text;
        Data.Player3 = player3.text;
        Data.Player4 = player4.text;
        Data.GameName = game_name.text;
        Data.PlayerNumber = int.Parse(player_number.text);
        Data.PlayerCardNumber = int.Parse(player_card_number.text);
        Data.TableCardNumber = int.Parse(table_card_number.text);
        if (need_animation.isOn == true) Data.NeedAnimation = true;
        else Data.NeedAnimation = false;
        if (need_draw_card.isOn == true) Data.NeedDrawCard = true;
        else Data.NeedDrawCard = false;

        SceneManager.LoadScene(2);
    }
}
