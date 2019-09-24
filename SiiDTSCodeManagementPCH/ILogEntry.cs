

namespace SiiDTSCodeManagementPCH
{
    interface ILogEntry
    {

        string DBID { get; set; }
        string id { get; set; }
        string state { get; set; }
        string type { get; }
        string host { get;  }
        long timestamp { get; set; }
        bool alert { get; set; }
       

        void RemapID();
    }
}
