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
            SlotSoundManager.bSndRef.PlaySoundEffect(ReferenceCenter.Ref.CommonMu.Container, SlotSoundManager.eAudioClip.Snd_ViewOpen.ToString());
            transform.Find("Gobj_BtnBox/UIBtn_ChatList/Image").gameObject.SetActive(true);
            transform.Find("Gobj_BtnBox/UIBtn_FriendList/Image").gameObject.SetActive(false);
            chatRoomList.OpenChatRoomList(account);
            friendList.CloseFriendList();
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 關閉聊天室"總"列表
        /// </summary>
        public void CloseListBox()
        {
            SlotSoundManager.bSndRef.PlaySoundEffect(ReferenceCenter.Ref.CommonMu.Container, SlotSoundManager.eAudioClip.Snd_ViewClose.ToString());
            gameObject.SetActive(false);
        }

        public void SwitchToChatListBox()
        {
            SlotSoundManager.bSndRef.PlaySoundEffect(ReferenceCenter.Ref.CommonMu.Container, SlotSoundManager.eAudioClip.Snd_ComClick1.ToString());
            transform.Find("Gobj_BtnBox/UIBtn_ChatList/Image").gameObject.SetActive(true);
            transform.Find("Gobj_BtnBox/UIBtn_FriendList/Image").gameObject.SetActive(false);
            chatRoomList.OpenChatRoomList(account);
            friendList.CloseFriendList();
        }

        public void SwitchToFriendListBox()
        {
            SlotSoundManager.bSndRef.PlaySoundEffect(ReferenceCenter.Ref.CommonMu.Container, SlotSoundManager.eAudioClip.Snd_ComClick1.ToString());
            transform.Find("Gobj_BtnBox/UIBtn_ChatList/Image").gameObject.SetActive(false);
            transform.Find("Gobj_BtnBox/UIBtn_FriendList/Image").gameObject.SetActive(true);
            chatRoomList.CloseChatRoomList();
            friendList.OpenFriendList(account);
        }
    }
}
