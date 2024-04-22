using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsMaker2Core.DataModels
{
    [Serializable]
    public class MessageArchive
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }
    }
}