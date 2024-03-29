﻿
using LiteDB;

namespace SiiDTSCodeManagementPCH
{

    class LogEntry: ILogEntry
    {

        [BsonId]
        public string DBID {get; set;}
        public string id {get; set;}
        public string state {get; set;}
        public long timestamp {get; set;}
        public string type { get { return null; } }
        public string host { get { return null; }}
        public bool alert {get; set;}

        public void RemapID()
        {
            DBID = id + "_" + state;
        }
    }


    
}
