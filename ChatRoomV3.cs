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
    public class ChatRoomV3 : MonoBehaviour
    {
#if __LOCAL_HOST__
        public const string CHAT_API_HOST = "http://localhost:50458"; 
#else
        public const string CHAT_API_HOST = "http://entrance10.mobiusdice.com.tw/friendAndChat";
#endif

        public const string GROUP_ID = "PrivateChatRoom";
        public GameObject prefab_PrivateRoom_Manifest;     
        public GameObject gobj_PublicRoom_Manifest;

        public GameObject manifestContent;
        public GameObject mask;

        public InputField chatInputField;
        Dictionary<string, PrivateChatRoomBean> mPrivateChatRoomDic;
        Dictionary<string, PrivateChatRoomBean> privateChatRoomDic
        {
            get
            {
                if (mPrivateChatRoomDic == null)
                {
                    mPrivateChatRoomDic = new Dictionary<string, PrivateChatRoomBean>();
                }
                return mPrivateChatRoomDic;
            }
        }


        Transform currentRoom;
        PlayerBean currentPlayerBean;
        const string url_push = "http://" + WebSocketController.HOST + "/Push";
        const string url_GetNewNotifiedRooms = CHAT_API_HOST + "/GetMyNewNotifyRooms";
        const string url_InvitePlayerToPrivateChatroom = CHAT_API_HOST + "/InvitePlayerToPrivateChatroom";
        const string url_PrivatePush = CHAT_API_HOST + "/PushPrivateMessage";
        const string url_DeleteNotify = CHAT_API_HOST + "/DeletePrivateRoomNotify";

        string mNickName;
        string account;

        public void OpenChatBox(string _account, string _nickName)
        {
            account = _account;
            mNickName = _nickName;
            gameObject.SetActive(true);
            gobj_PublicRoom_Manifest.GetComponent<PublicChatRoomBean>().Init(_account, _nickName);
            //currentRoom = ;
            if (currentRoom == null)
            {
                currentRoom = gobj_PublicRoom_Manifest.transform;
            }
            if (currentRoom != gobj_PublicRoom_Manifest.transform)
            {
                SetCurrentChatRoom(gobj_PublicRoom_Manifest.transform);
            }
            GetMyNewNotifiedRooms();

            
            //要給ListBox資料
            transform.Find("Gobj_ListBox").GetComponent<ListBox>().Init(_account);
        }

        public void CreateChatRoom(GameObject _searchBean)
        {
            SlotSoundManager.bSndRef.PlaySoundEffect(ReferenceCenter.Ref.CommonMu.Container, SlotSoundManager.eAudioClip.Snd_ComClick1.ToString());
            currentPlayerBean = _searchBean.GetComponent<PlayerBean>();
            Uri path = new Uri(url_InvitePlayerToPrivateChatroom);
            double lastUpdateTime = DateTime.UtcNow.AddHours(8).Ticks;
            HTTPRequest request = new HTTPRequest(path, HTTPMethods.Post, OnCreateChatRoomFinished);
            Dictionary<string, object> req = new Dictionary<string, object>();
            req.Add("account", account);
            req.Add("invitedPlayerAccount", currentPlayerBean.account);
            request.AddHeader("Content-Type", "application/json");
            request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req));
            request.Send();
            mask.SetActive(true);
        }


        private void OnCreateChatRoomFinished(HTTPRequest originalRequest, HTTPResponse response)
        {
            mask.SetActive(false);
            Debug.Log(response.DataAsText);
            transform.Find("Gobj_ListBox").GetComponent<ListBox>().CloseListBox();
            var table = JsonConvert.DeserializeObject<JObject>(response.DataAsText);
            string tableId = table.GetValue("tableId").ToString();
            if (!privateChatRoomDic.ContainsKey(tableId))
            {
                var room = Instantiate(prefab_PrivateRoom_Manifest, manifestContent.transform).GetComponent<PrivateChatRoomBean>();
                room.name = tableId;
                room.Init(tableId,account, mNickName, currentPlayerBean.account, currentPlayerBean.nickName, currentPlayerBean.iconIndex, currentPlayerBean.fbId, 0);
                privateChatRoomDic.Add(tableId, room);
                Generic.IconFetcher.SetIcon(room.transform.Find("Gobj_Pages/PageFrame/Icon/Icon_Chat").GetComponent<Image>(), currentPlayerBean.iconIndex, currentPlayerBean.fbId);
                room.gameObject.SetActive(true);
            }
            SetCurrentChatRoom(privateChatRoomDic[tableId].transform);
        }

        /// <summary>
        /// 取得所有新訊息聊天室
        /// </summary>
        public void GetMyNewNotifiedRooms()
        {
            Uri path = new Uri(url_GetNewNotifiedRooms);
            double lastUpdateTime = DateTime.UtcNow.AddHours(8).Ticks;
            HTTPRequest request = new HTTPRequest(path, HTTPMethods.Post, OnGetMyNewNotifiedRoomsFinish);
            Dictionary<string, object> req = new Dictionary<string, object>();
            req.Add("account", account);
            request.AddHeader("Content-Type", "application/json");
            request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req));
            request.Send();
            mask.SetActive(true);
        }

        /// <summary>
        /// 取得所有新訊息聊天室回傳結果
        /// </summary>
        private void OnGetMyNewNotifiedRoomsFinish(HTTPRequest originalRequest, HTTPResponse response)
        {
            Debug.Log(response.DataAsText);
            mask.SetActive(false);
            var result = JsonConvert.DeserializeObject<JObject>(response.DataAsText);
            var tableList = JsonConvert.DeserializeObject<List<JObject>>(result.GetValue("tableData").ToString());
            var IconDic = JsonConvert.DeserializeObject<Dictionary<string,JObject>>(result.GetValue("tableIcon").ToString());

            foreach (var table in tableList)
            {
                string tableId = table.GetValue("tableId").ToString();
                if (privateChatRoomDic.ContainsKey(tableId))
                {
                    privateChatRoomDic[tableId].CheckIsNew(double.Parse(table.GetValue("date").ToString()));
                }
                else
                {
                    var room = Instantiate(prefab_PrivateRoom_Manifest, manifestContent.transform).GetComponent<PrivateChatRoomBean>();
                    room.name = tableId;
                    string notifierAccount = table.GetValue("notifierAccount").ToString();

                    room.Init(tableId,account, mNickName, notifierAccount, table.GetValue("nickName").ToString(), (int)IconDic[notifierAccount].GetValue("iconIndex"), IconDic[notifierAccount].GetValue("fbId").ToString(), double.Parse(table.GetValue("date").ToString()));
                    privateChatRoomDic.Add(tableId, room);
                    Generic.IconFetcher.SetIcon(room.transform.Find("Gobj_Pages/PageFrame/Icon/Icon_Chat").GetComponent<Image>(), room.otherIconIndex, room.otherFbId);
                    room.gameObject.SetActive(true);
                }
            }

        }



        public void SetCurrentChatRoom(Transform _roomManifest)
        {
            SlotSoundManager.bSndRef.PlaySoundEffect(ReferenceCenter.Ref.CommonMu.Container, SlotSoundManager.eAudioClip.Snd_ComClick1.ToString());
            if (_roomManifest == currentRoom) return;

            //======先全部清乾淨
            gobj_PublicRoom_Manifest.GetComponent<PublicChatRoomBean>().publicChatRoomControl.gameObject.SetActive(false);
            gobj_PublicRoom_Manifest.transform.Find("Img_Selected").gameObject.SetActive(false);
            foreach (var room in privateChatRoomDic.Values)
            {
                if (room.PrivateChatRoomControl != null)
                {
                    room.PrivateChatRoomControl.gameObject.SetActive(false);
                }
                room.transform.Find("Img_Selected").gameObject.SetActive(false);
            }
            //======再打開現在的聊天室
            _roomManifest.transform.Find("Img_Selected").gameObject.SetActive(true);

            if (_roomManifest.Equals(gobj_PublicRoom_Manifest.transform))
            {               
                _roomManifest.GetComponent<PublicChatRoomBean>().publicChatRoomControl.gameObject.SetActive(true);
            }
            else
            {
                var privateBean = _roomManifest.GetComponent<PrivateChatRoomBean>();
                if (privateBean.PrivateChatRoomControl == null)
                {
                    var tempObject = privateBean.InstantiateScrollObject(privateBean);
                    
                }
                privateBean.PrivateChatRoomControl.gameObject.SetActive(true);
                _roomManifest.transform.Find("Img_IsNew").gameObject.SetActive(false);

            }
            currentRoom = _roomManifest.transform;
        }

        public void DeleteTheChatRoom(GameObject _beanObject)
        {
            SlotSoundManager.bSndRef.PlaySoundEffect(ReferenceCenter.Ref.CommonMu.Container, SlotSoundManager.eAudioClip.Snd_ComClick3.ToString());
            var bean = _beanObject.GetComponent<PrivateChatRoomBean>();          

            //傳送給伺服器刪除通知
            Uri path = new Uri(url_DeleteNotify);
            double lastUpdateTime = DateTime.UtcNow.AddHours(8).Ticks;
            HTTPRequest request = new HTTPRequest(path, HTTPMethods.Post);
            Dictionary<string, object> req = new Dictionary<string, object>();
            req.Add("account", account);
            req.Add("tableId", bean.tableId);
            request.AddHeader("Content-Type", "application/json");
            request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req));
            request.Send();

            //刪除生成物件
            if (bean.PrivateChatRoomControl != null)
            {
                Destroy(bean.PrivateChatRoomControl.gameObject);
            }
            privateChatRoomDic.Remove(bean.tableId);
            Destroy(_beanObject);
        }

        public void SendChatMessage()
        {
            SlotSoundManager.bSndRef.PlaySoundEffect(ReferenceCenter.Ref.CommonMu.Container, SlotSoundManager.eAudioClip.Snd_ComClick2.ToString());
            if (string.IsNullOrEmpty(chatInputField.text)) return;
            if (currentRoom == gobj_PublicRoom_Manifest.transform)
            {
                currentRoom.GetComponent<PublicChatRoomBean>().publicChatRoomControl.SendChatMessage(chatInputField.text, url_push);
            }
            else
            {
                currentRoom.GetComponent<PrivateChatRoomBean>().PrivateChatRoomControl.SendChatMessage(chatInputField.text, url_PrivatePush);
            }

            chatInputField.text = string.Empty;
        }

    }
}
