using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        //public bool isFriend;

        public double date;
        [HideInInspector]
        public PrivateChatRoomControl PrivateChatRoomControl;
        public GameObject prefab_PrivateRoom_Scroll;
        public Transform scrollContent;

        bool isPageSwitch;
        public void Init(string _tableId,string _myAccount, string _myNickName, string _otherAccount, string _otherNickName, double _date)
        {
            tableId = _tableId;
            myAccount = _myAccount;
            myNickName = _myNickName;
            otherAccount = _otherAccount;
            otherNickName = _otherNickName;
            UpdateDate(_date);
            transform.Find("Gobj_Pages/PageFrame/Txt_Title").GetComponent<Text>().text = _otherNickName;
        }

        public void UpdateDate(double _date)
        {
            string key = "PrivateChatRoom_" + tableId;
            date = _date;
            if (PlayerPrefs.HasKey(key))
            {
                Debug.Log("date : " + date);
                Debug.Log("double.Parse(PlayerPrefs.GetString(key)) : " + double.Parse(PlayerPrefs.GetString(key)));

                transform.Find("Img_IsNew").gameObject.SetActive(double.Parse(PlayerPrefs.GetString(key)) < date);               
            }
            else
            {
                transform.Find("Img_IsNew").gameObject.SetActive(true);
            }
            PlayerPrefs.SetString(key, date.ToString());
        }

        //public void SetIsFriend(bool _isFriend)
        //{
        //    isFriend = _isFriend;
        //    transform.Find("Gobj_Pages/PageFrame/UIBtn_AddFriend").gameObject.SetActive(!isFriend);
        //}
        
        public void SwitchPage()
        {
            uTweenPosition tween = transform.Find("Gobj_Pages/PageFrame").GetComponent<uTweenPosition>();
            if (!isPageSwitch)
            {              
                tween.PlayForward();
                transform.Find("Gobj_Pages/PageFrame/UIBtn_Arrow").localEulerAngles = new Vector3(0,0,180);
            }
            else
            {
                tween.PlayReverse();
                transform.Find("Gobj_Pages/PageFrame/UIBtn_Arrow").localEulerAngles = new Vector3(0, 0, 0);
            }
            isPageSwitch = !isPageSwitch;
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
