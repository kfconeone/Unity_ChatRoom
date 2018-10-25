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
    public class PrivateChatRoomControl : RoomTemplate, IChatRoom
    {

        public GameObject prefabSingleChatBarM;
        public GameObject prefabMultipleChatBarM;
        public GameObject prefabSingleChatBarO;
        public GameObject prefabMultipleChatBarO;

        public GameObject contentObject;
        public PrivateChatRoomBean myBean;
        double lastUpdateTime = 0;

        public override void OnError(string _message)
        {
            Debug.LogError(_message);
        }

        public override void OnMessage(string _message)
        {
            Debug.LogError(_message);
            string response = JsonConvert.DeserializeObject<JObject>(_message).GetValue("pushObject").ToString();
            JObject pushObject = JsonConvert.DeserializeObject<JObject>(response);

            if (pushObject.GetValue("account").ToString().Equals(myBean.myAccount))
            {
                GenerateText(pushObject.GetValue("nickName").ToString(), pushObject.GetValue("content").ToString(), (int)pushObject.GetValue("iconIndex"), pushObject.GetValue("fbId").ToString(), true);
            }
            else
            {
                GenerateText(pushObject.GetValue("nickName").ToString(), pushObject.GetValue("content").ToString(), (int)pushObject.GetValue("iconIndex"), pushObject.GetValue("fbId").ToString(), false);

            }
            lastUpdateTime = (double)pushObject.GetValue("date");
            Debug.Log("lastUpdateTime : " + lastUpdateTime.ToString());
            PlayerPrefs.SetString("PrivateChatRoom_" + myBean.tableId, lastUpdateTime.ToString());
            Invoke("RefreshCanvas", 0.01f);
        }

        public override void OnSubscribeFinished(string _message)
        {
            Debug.Log(_message);
            JToken detailToken = JsonConvert.DeserializeObject<JObject>(_message).GetValue("detail");
            if (!detailToken.HasValues) return;
            List<JObject> detail = JsonConvert.DeserializeObject<List<JObject>>(detailToken.ToString());

            foreach (JObject jobj in detail)
            {
                double tempDate = (double)jobj.GetValue("date");
                if (tempDate > lastUpdateTime)
                {

                    if (jobj.GetValue("account").ToString().Equals(myBean.myAccount))
                    {
                        GenerateText(jobj.GetValue("nickName").ToString(), jobj.GetValue("content").ToString(), (int)jobj.GetValue("iconIndex"), jobj.GetValue("fbId").ToString(), true);
                    }
                    else
                    {
                        GenerateText(jobj.GetValue("nickName").ToString(), jobj.GetValue("content").ToString(), (int)jobj.GetValue("iconIndex"), jobj.GetValue("fbId").ToString(), false);
                    }
                    lastUpdateTime = tempDate;
                }
                Invoke("RefreshCanvas", 0.01f);

            }
            Debug.Log("lastUpdateTime : " + lastUpdateTime.ToString());
            PlayerPrefs.SetString("PrivateChatRoom_" + myBean.tableId, lastUpdateTime.ToString());

        }

        void RefreshCanvas()
        {
            Canvas.ForceUpdateCanvases();
            transform.Find("Scroll_Chatting/Content").GetComponent<HorizontalOrVerticalLayoutGroup>().enabled = false;
            transform.Find("Scroll_Chatting/Content").GetComponent<HorizontalOrVerticalLayoutGroup>().enabled = true;
        }

        public void SendChatMessage(string _message, string _url, int _myIconIndex, string _myFbId)
        {
            if (string.IsNullOrEmpty(_message))
            {
                return;
            }
            Uri path = new Uri(_url);
            double date = DateTime.UtcNow.AddHours(8).Ticks;
            HTTPRequest request = new HTTPRequest(path, HTTPMethods.Post, OnSendMsgFinish);

            Dictionary<string, object> req = new Dictionary<string, object>();
            req.Add("account", myBean.myAccount);
            req.Add("nickName", myBean.myNickName);
            req.Add("iconIndex", _myIconIndex);
            req.Add("fbId", _myFbId);
            req.Add("tableId", roomName);
            req.Add("message", _message);

            request.AddHeader("Content-Type", "application/json");
            request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req));
            request.Send();
        }

        private void OnSendMsgFinish(HTTPRequest originalRequest, HTTPResponse response)
        {
            if (response == null || response.StatusCode != 200)
            {
                Debug.LogError("與伺服器端連接失敗" + response.DataAsText);
                return;
            }

            Debug.Log("聊天室傳送結束訊息：" + response.DataAsText);
            JObject res = JsonConvert.DeserializeObject<JObject>(response.DataAsText);
            string result = res.GetValue("result").ToString();
        }



        void GenerateText(string _nickName, string _content,int _iconIndex,string _fbId, bool _isMe)
        {

            GameObject tempPrefab;
            if (Encoding.Default.GetByteCount(_content) < 40)
            {
                if (_isMe) tempPrefab = prefabSingleChatBarM;
                else tempPrefab = prefabSingleChatBarO;
            }
            else
            {
                if (_isMe) tempPrefab = prefabMultipleChatBarM;
                else tempPrefab = prefabMultipleChatBarO;

            }

            Transform text = Instantiate(tempPrefab, contentObject.transform).transform;
            text.Find("Gobj_Info/UITxt_Name").GetComponent<Text>().text = _nickName;
            Generic.IconFetcher.SetIcon(text.Find("Gobj_Info/UIImg_Icon").GetComponent<Image>(), _iconIndex, _fbId);
            text.Find("UIImg_MsgBG/Text").GetComponent<Text>().text = _content;
            text.gameObject.SetActive(true);
            if (text.GetComponent<HorizontalOrVerticalLayoutGroup>() != null)
            {
                Canvas.ForceUpdateCanvases();
                text.GetComponent<HorizontalOrVerticalLayoutGroup>().enabled = false;
                text.GetComponent<HorizontalOrVerticalLayoutGroup>().enabled = true;

            }
            Canvas.ForceUpdateCanvases();
            text.Find("UIImg_MsgBG").GetComponent<HorizontalOrVerticalLayoutGroup>().enabled = false;
            text.Find("UIImg_MsgBG").GetComponent<HorizontalOrVerticalLayoutGroup>().enabled = true;

        }
    }
}
