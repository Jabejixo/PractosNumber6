using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMaSsAGeR.MVVM.Model
{
    public class Message
    {
        public string ClientName { get; set; }
        public string MessageText { get; set; }
        public bool IsOwn { get; set; }
        public MessageType Type { get; set; }
    }

    public enum MessageType
    {
        ToServer,
        Text,
        Error,
        Info
    }
}

