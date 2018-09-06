using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kfc.ChatRoom
{
    public class ListBox : MonoBehaviour
    {
        string account;
        public ChatRoomList chatRoomList;
        public FriendList friendList;

        public void Init(string _account)
        {
            account = _account;
        }
        /// <summary>
        /// 開啟聊天室"總"列表
        /// </summary>
        public void OpenListBox()
        {
            chatRoomList.OpenChatRoomList();
            friendList.CloseFriendList();
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 關閉聊天室"總"列表
        /// </summary>
        public void CloseListBox()
        {
            gameObject.SetActive(false);
        }

        public void SwitchToChatListBox()
        {
            chatRoomList.OpenChatRoomList();
            friendList.CloseFriendList();
        }

        public void SwitchToFriendListBox()
        {
            chatRoomList.CloseChatRoomList();
            friendList.OpenFriendList(account);
        }
    }
}
