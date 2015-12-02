using Microsoft.ServiceBus.Messaging;
using SmartMessageServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace K2Field.SmartMessages.Listeners
{
    public class ASBSubscriptionListener : SmartMessageListenerInterface
    {
        private SmartMessageFrameworkContext _frameWorkContext;
        private Boolean _isRunning = false;
        private String _listenerInstanceFQN;
        AutoResetEvent _threadEvent = new AutoResetEvent(false);

        private string TopicName;
        private string SubscriptionName;
        private SubscriptionClient Client;

        public void InitWithConfig(SmartMessageFrameworkContext frameworkContext, string listenerInstanceFQN, string configString)
        {
            _frameWorkContext = frameworkContext;

            dynamic config = Newtonsoft.Json.JsonConvert.DeserializeObject(configString);
            string connectionString = config.connectionString;
            TopicName = config.topicName;
            SubscriptionName = config.subscriptionName;

            Console.WriteLine("--Config {0}: {1} {2} {3}", _listenerInstanceFQN, TopicName, SubscriptionName, connectionString);

            //var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            Client = SubscriptionClient.CreateFromConnectionString(connectionString, TopicName, SubscriptionName);
            _listenerInstanceFQN = listenerInstanceFQN;
        }

        public void RunListener(object frameworkContext)
        {
            _isRunning = true;
            Console.WriteLine("--Starting: {0}", _listenerInstanceFQN);

            
            Client.OnMessage((receivedMessage) =>
            {
                try
                {
                    BrokeredMessage ClonedMessage = receivedMessage.Clone(); 

                    Console.WriteLine("--Message Received {0}: {1}", _listenerInstanceFQN, receivedMessage.CorrelationId);

                    string JsonEvent = string.Empty;
                    string ContentType = string.Empty;

                    ContentType = receivedMessage.ContentType;

                    string brokeredMessageJson = Newtonsoft.Json.JsonConvert.SerializeObject(receivedMessage);

                    dynamic m = new System.Dynamic.ExpandoObject();
                    m.BrokeredMessage = Newtonsoft.Json.JsonConvert.DeserializeObject(brokeredMessageJson);
                    
                    if (!receivedMessage.IsBodyConsumed)
                    {
                        using (Stream stream = receivedMessage.GetBody<Stream>())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                JsonEvent = reader.ReadToEnd();
                            }
                        }
                    }

                    if (JsonEvent.StartsWith("{"))
                    {
                        m.Body = Newtonsoft.Json.JsonConvert.DeserializeObject(JsonEvent);
                    }
                    else
                    {
                        m.Body = JsonEvent;
                    }

                    string mJson = Newtonsoft.Json.JsonConvert.SerializeObject(m);

                    Console.WriteLine("--Message Body Read {0}:\n\n {1}", _listenerInstanceFQN, mJson);

                    //ENQUEUE INCOMING MESSAGES
                    String msgID = _frameWorkContext.EnqueueSmartMessageForProcessing(_listenerInstanceFQN, mJson);
                    

                    // Remove message from subscription
                    receivedMessage.Complete();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("--Message Processing Exception {0}: {1}", _listenerInstanceFQN, ex.GetBaseException().Message);
                    try
                    {
                        receivedMessage.Abandon();
                    }
                    catch { }
                }
            });
        }

        public void StopListener()
        {
            Console.WriteLine("--Stopping: {0}", _listenerInstanceFQN);
            Client.Close();
            _isRunning = false;
            _threadEvent.Set();
        }
    }
}
