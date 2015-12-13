using SmartMessageServer;
using SourceCode.Workflow.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartMessages.Processor
{
    public class StartK2WorkflowProcessor : SmartMessageProcessorInterface
    {

        private string _k2ConnectString = "";
        private string _k2MessageDataField = "";
        private string _k2WorkflowName = "";

        public void InitWithConfig(SmartMessageFrameworkContext frameworkContext, string targetProcessorInstanceFQN, string configString)
        {
            dynamic config = Newtonsoft.Json.JsonConvert.DeserializeObject(configString);
            _k2ConnectString = config.K2ConnectionString;
            _k2MessageDataField = config.MessageDataField;
            _k2WorkflowName = config.Workflow;
        }

        public void ProcessMessage(string MessageID, string MessageData, string sourceListenerInstanceFQN)
        {
            Console.WriteLine("--Processing Message: {0}", sourceListenerInstanceFQN);

            try
            {
                ASBMessage msg = Newtonsoft.Json.JsonConvert.DeserializeObject<ASBMessage>(MessageData);
                string body = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Body);

                using (Connection k2con = new Connection())
                {
                    //{ "Workflow" : "CSR Workflow\\CSR Product PARCAR", "MessageDataField": "Event Data", "K2ConnectionString" : "Integrated=True;IsPrimaryLogin=True;Authenticate=True;EncryptedPassword=False;Host=k2.denallix.com;Port=5252" }

                    string constring = _k2ConnectString;
                    
                    k2con.Open("localhost", constring);
                    ProcessInstance pi = k2con.CreateProcessInstance(_k2WorkflowName);
                    pi.DataFields[_k2MessageDataField].Value = body;
                    k2con.StartProcessInstance(pi);

                    k2con.Close();
                }


            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }

        }
    }
}
