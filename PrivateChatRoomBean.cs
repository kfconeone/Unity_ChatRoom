﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using uTools;

namespace Kfc.ChatRoom
{
    public class PrivateChatRoomBean : MonoBehaviour {

        public string tableId;
        public string myAccount;
        public string myNickName;
        public string otherAccount;
        public string otherNickName;
        public int otherIconIndex;
        public string otherFbId;
        //public bool isFriend;

        public double date;
        [HideInInspector]
        public PrivateChatRoomControl PrivateChatRoomControl;
        public GameObject prefab_PrivateRoom_Scroll;
        public Transform scrollContent;

        
        public void Init(string _tableId,string _myAccount, string _myNickName, string _otherAccount, string _otherNickName,int _otherIconIndex,string _otherFbId, double _date)
        {
            tableId = _tableId;
            myAccount = _myAccount;
            myNickName = _myNickName;
            otherAccount = _otherAccount;
            otherNickName = _otherNickName;
            otherIconIndex = _otherIconIndex;
            otherFbId = _otherFbId;
            CheckIsNew(_date);
            transform.Find("Gobj_Pages/PageFrame/Gobj_Text/Txt_Title").GetComponent<Text>().text = _otherNickName;
            
        }


        public void CheckIsNew(double _date)
        {
            string key = "PrivateChatRoom_" + tableId;
            GameObject isNewIcon = transform.Find("Img_IsNew").gameObject;
            date = _date;
            if (PlayerPrefs.HasKey(key))
            {
                Debug.Log("date : " + date);
                Debug.Log("double.Parse(PlayerPrefs.GetString(key)) : " + double.Parse(PlayerPrefs.GetString(key)));

                isNewIcon.SetActive(double.Parse(PlayerPrefs.GetString(key)) < date);               
            }
            else
            {
                isNewIcon.SetActive(true);
            }

            if (isNewIcon.activeSelf == true)
            {
                isNewIcon.GetComponent<uTweenScale>().ResetToBeginning();
                isNewIcon.GetComponent<uTweenScale>().PlayForward();
            }
            //PlayerPrefs.SetString(key, date.ToString());
        }

        

        public GameObject InstantiateScrollObject(PrivateChatRoomBean _bean)
        {
            var scrollObject = Instantiate(prefab_PrivateRoom_Scroll, scrollContent);
            scrollObject.name = tableId;
            PrivateChatRoomControl = scrollObject.GetComponent<PrivateChatRoomControl>();
            PrivateChatRoomControl.groupId = ChatRoomV3.GROUP_ID;
            PrivateChatRoomControl.roomName = tableId;
            PrivateChatRoomControl.myBean = _bean;
            return scrollObject;
        }
    }
}
