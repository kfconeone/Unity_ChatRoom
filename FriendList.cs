using BestHTTP;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Xefier.Threading.Asynchronous;

namespace Kfc.ChatRoom
{
    public class FriendList : MonoBehaviour
    {
        public GameObject mask;
        public GameObject prefabFriend;
        public Transform friendContent;
        string account;

        public const string url_GetMyFriends = ChatRoomV3.CHAT_API_HOST + "/GetMyFriends";


        /// <summary>
        /// 開啟好友列表
        /// </summary>
        public void OpenFriendList(string _account)
        {
            gameObject.SetActive(true);
            account = _account;
            GetMyFriends();
        }

        /// <summary>
        /// 關閉好友列表
        /// </summary>
        public void CloseFriendList()
        {
            //DestroyAllSearchedPlayers();
            gameObject.SetActive(false);
        }


        void GetMyFriends()
        {
            Uri path = new Uri(url_GetMyFriends);
            HTTPRequest request = new HTTPRequest(path, HTTPMethods.Post, OnGetMyFriendsFinished);

            Dictionary<string, object> req = new Dictionary<string, object>();
            req.Add("account", account);
            request.AddHeader("Content-Type", "application/json");
            request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req));
            request.Send();
            mask.SetActive(true);
        }

        private void OnGetMyFriendsFinished(HTTPRequest originalRequest, HTTPResponse response)
        {
            mask.SetActive(false);
            Debug.Log(response.DataAsText);
            Dictionary<string, object> jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.DataAsText);
            if (!jsonResponse["result"].ToString().Contains("000"))
            {
                return;
            }
            var friendList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonResponse["friendList"].ToString());

            if (friendContent.childCount > 0)
            {
                List<GameObject> oldContentList = new List<GameObject>();
                for (int i = 0; i < friendContent.childCount; ++i)
                {
                    oldContentList.Add(friendContent.GetChild(i).gameObject);
                }

                foreach (var child in oldContentList)
                {
                    Destroy(child);
                }
            }

            foreach (var friend in friendList)
            {
                var task = Async.Instantiate(prefabFriend, friendContent, false);
                task.Ready += (_task) =>
                {
                    var friendObj = _task.Result;
                    friendObj.SetActive(true);
                    friendObj.name = friend["account"].ToString();
                    friendObj.GetComponent<PlayerBean>().Init(friend["account"].ToString(), friend["name"].ToString(), friend["fbId"].ToString(), int.Parse(friend["iconIndex"].ToString()), int.Parse(friend["vipLevel"].ToString()), friend["saySomething"].ToString());
                    friendObj.transform.Find("UITxt_Name").GetComponent<Text>().text = friend["name"].ToString();
                    friendObj.transform.Find("UIImg_TalkMask/UITxt_Talking").GetComponent<Text>().text = friend["saySomething"].ToString();
                    Generic.IconFetcher.SetIcon(friendObj.transform.Find("UIImg_Icon").GetComponent<Image>(), (int)friend["iconIndex"], friend["fbId"].ToString());

                };
             
            }

        }
    }
}