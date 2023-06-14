using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMaSsAGeR.MVVM.Model
{
    public enum TypeMessage
    {
        ToServer,
        Text,
        Error,
        Info
    }
    public class Message
    {
        public string ClientName { get; set; }
        public string MessageText { get; set; }
        public bool ItYou { get; set; }
        public TypeMessage Type { get; set; }
    }

    
}

