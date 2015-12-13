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
    public class ASBMessageSignalRProcessor : SmartMessageProcessorInterface
    {
        private string _notificationUrl = string.Empty;
        private string _defaultGroupName = string.Empty;

        public void InitWithConfig(SmartMessageFrameworkContext frameworkContext, string targetProcessorInstanceFQN, string configString)
        {
            dynamic config = Newtonsoft.Json.JsonConvert.DeserializeObject(configString);
            _notificationUrl = config.notificationUrl;
            _defaultGroupName = config.defaultGroupName;

        }

        public void ProcessMessage(string MessageID, string MessageData, string sourceListenerInstanceFQN)
        {
            try
            {

                Console.WriteLine("--Processing Message: {0}", sourceListenerInstanceFQN);

                ASBMessage msg = Newtonsoft.Json.JsonConvert.DeserializeObject<ASBMessage>(MessageData);

                string group = !string.IsNullOrWhiteSpace(msg.BrokeredMessage.To) ? _defaultGroupName + "," + msg.BrokeredMessage.To : _defaultGroupName;

                string url = _notificationUrl;
                url += "?group=" + group;

                string name = !string.IsNullOrWhiteSpace(msg.BrokeredMessage.Label) ? msg.BrokeredMessage.Label : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                url += "&name=" + name;

                string bodyString = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Body);
                url += "&message=" + bodyString;

                url = string.Format("{0}&origin={1}&source={2}&category={3}&type={4}&to={5}&from={6}&data={7}&datatype={8}&actionurl={9}", url, "SmartMessages", sourceListenerInstanceFQN, msg.BrokeredMessage.ContentType, "info", msg.BrokeredMessage.To, "", "", "", "");

                Console.WriteLine("\n\n");
                Console.WriteLine("--SignalR url: {0}", url);
                Console.WriteLine("\n\n");

                CallService(null, url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }
        }


        public async void CallService(ICredentials credentials, string uri)
        {
            HttpWebRequest request = null;
            //NotificationObject item;
            string json = string.Empty;
            try
            {
                string RequestUri = string.Format("{0}", uri);
                request = (HttpWebRequest)WebRequest.Create(RequestUri);
                request.Method = "GET";
                request.Accept = "application/json";
                //request.Credentials = credentials;

                using (HttpWebResponse Response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    //using (Stream st = Response.GetResponseStream())
                    //{
                    //    using (StreamReader sr = new StreamReader(st))
                    //    {
                    //        json = sr.ReadToEnd();
                    //        item = Newtonsoft.Json.JsonConvert.DeserializeObject<NotificationObject>(json);
                    //    }
                    //}
                }
            }
            catch (WebException wex)
            {
                Console.WriteLine(wex.GetBaseException().Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }
            finally
            {
                request = null;
            }
            //return item;
        }
    }





    
}
