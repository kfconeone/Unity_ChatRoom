using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChatRoom : MonoBehaviour {

    public Kfc.PublicChatRoomV2 chatRoom;
	// Use this for initialization
	void Start () {
        chatRoom.OpenChatBox("GM0000000006","鄉村有機牛");
    }
	
}
