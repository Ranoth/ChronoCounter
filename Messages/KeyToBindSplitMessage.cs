using CommunityToolkit.Mvvm.Messaging.Messages;
using WPFUtilsBox.HotKeyer;

namespace ChronoCounter.Messages
{
    class KeyToBindSplitMessage : ValueChangedMessage<HotKey>
    {
        public KeyToBindSplitMessage(HotKey value) : base(value)
        {
        }
    }
}
