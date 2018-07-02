using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BestHTTP;
using System.Text;

namespace Kfc
{
    public class PublicChatRoomV2 : RoomTemplate
    {
        public GameObject gobj_PublicChatBox;
        public GameObject prefab_Text;
        public GameObject mask;

        public InputField chatInputField;

        public Transform trans_PublicChatBoxContent;
#if __Localhost__
        const string HOST = "localhost:8081";
#else
        const string HOST = "entrance10.mobiusdice.com.tw:700";
#endif
        const string url_push = "http://" + HOST + "/Push";


        string mNickName;
        string account;
        double lastUpdateTime = 0;
        public void OpenChatBox(string _account,string _nickName)
        {
            account = _account;
            mNickName = _nickName;
            gameObject.SetActive(true);
            gobj_PublicChatBox.SetActive(true);
        }

        public void CloseChatBox()
        {

            gameObject.SetActive(false);
            gobj_PublicChatBox.SetActive(false);
        }

        public override void OnError(string _message)
        {
            Debug.LogError(_message);
        }

        public override void OnMessage(string _message)
        {
            Debug.LogError(_message);
            string response = JsonConvert.DeserializeObject<JObject>(_message).GetValue("pushObject").ToString();
            JObject pushObject = JsonConvert.DeserializeObject<JObject>(response);
            if (pushObject.GetValue("account").ToString().Equals(account))
            {
                GenerateText(pushObject.GetValue("nickName").ToString(), pushObject.GetValue("content").ToString(), "6DC9FFFF", "ffffffff");
            }
            else
            {
                GenerateText(pushObject.GetValue("nickName").ToString(), pushObject.GetValue("content").ToString(), "FFB26DFF", "ffffffff");

            }
        }

        public override void OnSubscribeFinished(string _message)
        {
            Debug.LogError(_message);
            JToken detailToken = JsonConvert.DeserializeObject<JObject>(_message).GetValue("detail");

            if (!detailToken.HasValues) return;
            List<JObject> detail = JsonConvert.DeserializeObject<List<JObject>>(detailToken.ToString());

            foreach (JObject jobj in detail)
            {
                double tempDate = (double)jobj.GetValue("date");
                if (tempDate > lastUpdateTime)
                {
                    if (jobj.GetValue("account").ToString().Equals(account))
                    {
                        GenerateText(jobj.GetValue("nickName").ToString(), jobj.GetValue("content").ToString(), "6DC9FFFF", "ffffffff");
                    }
                    else
                    {
                        GenerateText(jobj.GetValue("nickName").ToString(), jobj.GetValue("content").ToString(), "FFB26DFF", "ffffffff");
                    }
                    lastUpdateTime = tempDate;
                }              
            }
        }


        public void SendChatMessage()
        {
            if (string.IsNullOrEmpty(chatInputField.text))
            {
                return;
            }
            Uri path = new Uri(url_push);
            double lastUpdateTime = DateTime.UtcNow.AddHours(8).Ticks;
            HTTPRequest request = new HTTPRequest(path, HTTPMethods.Post, OnSendMsgFinish);
            Dictionary<string, object> tempChat = new Dictionary<string, object>();
            tempChat.Add("account", account);
            tempChat.Add("nickName", mNickName);
            tempChat.Add("content", chatInputField.text);
            tempChat.Add("date", lastUpdateTime);

            Dictionary<string, object> req = new Dictionary<string, object>();
            req.Add("groupId", groupId);
            req.Add("tableId", roomName);
            req.Add("pushObject", tempChat);

            request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req));
            request.Send();
        }

        private void OnSendMsgFinish(HTTPRequest originalRequest, HTTPResponse response)
        {
            if (response == null || response.StatusCode != 200)
            {
                Debug.LogError("與伺服器端連接失敗");
                return;
            }


            Debug.Log("聊天室傳送結束訊息：" + response.DataAsText);
            chatInputField.text = string.Empty;
            JObject res = JsonConvert.DeserializeObject<JObject>(response.DataAsText);
            string result = res.GetValue("result").ToString();
            if (!result.Contains("000"))
            {
                GenerateText("系統提醒", "網路不穩，正在重新連接聊天室...", "ffff00ff", "ffff00ff");
            }
        }

        void GenerateText(string _nickName, string _content,string _nameColorCode, string _contentColorCode)
        {
            Text txt = Instantiate(prefab_Text, trans_PublicChatBoxContent).GetComponent<Text>();
            txt.transform.SetAsLastSibling();
            txt.gameObject.name = txt.transform.GetSiblingIndex().ToString();
            txt.text = string.Format("<color=#{0}>{1}：</color><color=#{2}>{3}</color>", _nameColorCode, _nickName, _contentColorCode, _content);
            txt.gameObject.SetActive(true);
        }
    }

}
