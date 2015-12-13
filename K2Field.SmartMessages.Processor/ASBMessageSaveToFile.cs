using SmartMessageServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartMessages.Processor
{
    public class ASBMessageSaveToFile : SmartMessageProcessorInterface
    {
        private string _directory = string.Empty;
        private string _defaultGroupName = string.Empty;

        public void InitWithConfig(SmartMessageFrameworkContext frameworkContext, string targetProcessorInstanceFQN, string configString)
        {
            dynamic config = Newtonsoft.Json.JsonConvert.DeserializeObject(configString);
            _directory = config.directory;
        }

        public void ProcessMessage(string MessageID, string MessageData, string sourceListenerInstanceFQN)
        {
            try
            {
                Console.WriteLine("--Processing Message: {0}", sourceListenerInstanceFQN);

                ASBMessage msg = Newtonsoft.Json.JsonConvert.DeserializeObject<ASBMessage>(MessageData);
                string body = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Body);

                string filename = DateTime.UtcNow.ToString("yyyMMdd-HHmmss-" + MessageID + ".txt");
                DirectoryInfo di = null;
                if (!Directory.Exists(_directory))
                {
                    di = Directory.CreateDirectory(_directory);
                }
                else
                {
                    di = new DirectoryInfo(_directory);
                }

                string fullpath = di.FullName + @"\" + filename;
                Console.WriteLine("--Writing File: {0}", fullpath);

                File.WriteAllText(fullpath, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
