using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using LiteDB;
using Serilog;

namespace SiiDTSCodeManagementPCH
{
    class Program
    {
        

        static void Main(string[] args)
        {
            // DB related const
            const string DATABASE_FILE_NAME_PREFIX = "LogsDB_";
            const string DATABASE_FILE_NAME_SUFFIX = ".db";
            const string TABLE_NAME = "Logs";

            const string LOG_FILENAME = "AppLog.log";

            //Serilog preparation

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(LOG_FILENAME, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .CreateLogger();

            string sFilePath;
            try
            {
                sFilePath = args[0];
            }
            catch (Exception)
            {
                Log.Error("Required source file parameter has not been provided");
                return;
            }
            
            if (!File.Exists(sFilePath))
            {
                //System.Diagnostics.Debug.WriteLine("{0:yyyy.mm.dd HH.mm.ss.ffffff} : ERR : FILE_DO_NOT_EXIST : {1} ", DateTime.Now, sFilePath);
                Log.Error("Input file doesn't exist", sFilePath);
                return ;
            }

            Log.Information("Loading file {0}", sFilePath);
            //Log.Information("{0:yyyy.mm.dd HH.mm.ss.ffffff} : ERR : FILE_DO_NOT_EXIST : {1} ", DateTime.Now, sFilePath);


            // preparing buffer dictionary
            Dictionary<string, ILogEntry> entriesDict = new Dictionary<string, ILogEntry>();

            //Preparing Lite Database File Name
            string sFileName = Path.GetFileNameWithoutExtension(sFilePath);
            string sLiteDbFileName = DATABASE_FILE_NAME_PREFIX + sFileName + DATABASE_FILE_NAME_SUFFIX;

            using (var reader = new StreamReader(sFilePath, true))
            using (var db = new LiteDatabase(sLiteDbFileName))
            {
                try
                {
                    //Get table (existing or create automatically)
                    LiteCollection<ILogEntry> dbTable = db.GetCollection<ILogEntry>(TABLE_NAME);

                    while (!reader.EndOfStream)
                    {
                        string sLine = reader.ReadLine();
                        Log.Information("Processing line : {0}", sLine);
                        ILogEntry logEntry;
                        if (sLine.Contains("\"host\""))
                        {
                            logEntry = JsonConvert.DeserializeObject<LogEntryExt>(sLine);
                        }
                        else
                        {
                            logEntry = JsonConvert.DeserializeObject<LogEntry>(sLine);
                        }

                        logEntry.RemapID();                         //Update ID for LiteDB

                        if (entriesDict.ContainsKey(logEntry.id))   // counterpart alredy in buffer
                        {
                            ILogEntry preEntry = entriesDict.First(x => x.Value.id == logEntry.id).Value;
                            if (preEntry.state == "FINISHED")
                            {
                                // FINISHED was before STARTED so Change order before saving to db
                                ILogEntry tempEntry = preEntry;
                                preEntry = logEntry;
                                logEntry = tempEntry;
                            }
                            //check for time span over 4ms
                            if (logEntry.timestamp - preEntry.timestamp > 4)
                            {
                                logEntry.alert = true;
                                preEntry.alert = true;
                                
                            }
                            // writing to db
                            try
                            {
                                var idBegin = dbTable.Insert(logEntry);
                                var idEnd = dbTable.Insert(preEntry);
                            }
                            catch (Exception)
                            {
                                Log.Error("Event with ID = {0} already exists in the database", logEntry.id);
                            }

                            // removing item with such key from temp dictionary
                            entriesDict.Remove(logEntry.id);
                        }
                        else  // counterpart not found yet
                        {
                            entriesDict.Add(logEntry.id, logEntry);     //add to the buffer 
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "General processing error occured at line");
                }
            }

            Log.CloseAndFlush();

            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Press Any Key");
            Console.ReadKey();
        }
    }
}
