using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BestHTTP;
using System.Text;

namespace Kfc.ChatRoom
{
    public class PublicChatRoomBean : MonoBehaviour
    {

        public string mNickName;
        public string account;
        public PublicChatRoomControl publicChatRoomControl;
        public void Init(string _account, string _nickName)
        {
            account = _account;
            mNickName = _nickName;
            gameObject.SetActive(true);

        }
    }
}
