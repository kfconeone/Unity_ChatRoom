using BestHTTP;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Kfc.ChatRoom
{
    public class ChatRoomList : MonoBehaviour
    {
        string account;
        public GameObject mask;
        public GameObject prefab_searchedPlayer;
        public Transform trans_SearchContent;
        public InputField input_NickName;
        public Text txt_SearchResult;


        string url_SearchPlayerWithNickName = ChatRoomV3.CHAT_API_HOST + "/GetPlayerStatusByPartialNickName";

        /// <summary>
        /// 開啟聊天室列表
        /// </summary>
        public void OpenChatRoomList(string _account)
        {
            account = _account;
            txt_SearchResult.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 關閉聊天室列表
        /// </summary>
        public void CloseChatRoomList()
        {
            //DestroyAllSearchedPlayers();
            txt_SearchResult.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 搜尋玩家
        /// </summary>
        public void SearchPlayerByNickName()
        {
            if (string.IsNullOrEmpty(input_NickName.text)) return;

            Uri path = new Uri(url_SearchPlayerWithNickName);
            HTTPRequest request = new HTTPRequest(path, HTTPMethods.Post, OnSearchPlayerByNickNameFinished);

            Dictionary<string, object> req = new Dictionary<string, object>();
            req.Add("account", account);
            req.Add("nickName", input_NickName.text);


            request.AddHeader("Content-Type", "application/json");
            request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req));
            request.Send();
            mask.SetActive(true);
            txt_SearchResult.gameObject.SetActive(false);
        }

        private void OnSearchPlayerByNickNameFinished(HTTPRequest originalRequest, HTTPResponse response)
        {
            mask.SetActive(false);
            txt_SearchResult.gameObject.SetActive(true);
            string tempResult = "\" {0} \"的搜尋結果 : 共有 {1} 筆結果";

            Debug.Log(response.DataAsText);
            Dictionary<string,object> jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.DataAsText);
            if (!jsonResponse["result"].ToString().Contains("000"))
            {
                txt_SearchResult.text = string.Format(tempResult, input_NickName.text, "0");
                return;
            }

            List<Dictionary<string, object>> players = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonResponse["players"].ToString());
            txt_SearchResult.text = string.Format(tempResult, input_NickName.text, players.Count);

            DestroyAllSearchedPlayers();

            foreach (var player in players)
            {
                var bean = Instantiate(prefab_searchedPlayer, trans_SearchContent).GetComponent<PlayerBean>();
                bean.Init(player["account"].ToString(), player["name"].ToString(), player["fbId"].ToString(), int.Parse(player["vipLevel"].ToString()), int.Parse(player["iconIndex"].ToString()), player["saySomething"].ToString());

                bean.txt_nickName.text = bean.nickName;
                bean.txt_saySomething.text = bean.saySomething;
                Generic.IconFetcher.SetIcon(bean.transform.Find("UIImg_Icon").GetComponent<Image>(), bean.iconIndex, bean.fbId);

                bean.gameObject.SetActive(true);
            }
        }

        void DestroyAllSearchedPlayers()
        {
            if (trans_SearchContent.childCount == 0) return;

            List<Transform> tempPlayers = new List<Transform>();
            for (int i = 0; i < trans_SearchContent.childCount; ++i)
            {
                tempPlayers.Add(trans_SearchContent.GetChild(i));
            }

            foreach (var player in tempPlayers)
            {
                Destroy(player.gameObject);
            }

            tempPlayers.Clear();
        }

    }
}
