using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace SiiDTSCodeManagementPCH
{

    class LogEntry: ILogEntry
    {

        [BsonId]
        public string DBID { get; set; }
        public string id { get; set; }
        public string state { get; set; }
        public long timestamp {get; set;}
        public string type { get => null;  }
        public string host { get => null;  }
        public bool alert { get; set; }

        public void RemapID()
        {
            DBID = id + "_" + state;
        }
    }


    
}
