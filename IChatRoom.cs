
namespace Kfc.ChatRoom
{
    public interface IChatRoom
    {
        void SendChatMessage(string _message,string _url, int _myIconIndex, string _myFbId);
    }
}
