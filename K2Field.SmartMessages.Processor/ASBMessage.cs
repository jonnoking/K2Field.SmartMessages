using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartMessages.Processor
{
    public class ASBMessage
    {
        public Brokeredmessage BrokeredMessage { get; set; }
        public dynamic Body { get; set; }
    }

    public class Brokeredmessage
    {
        public string CorrelationId { get; set; }
        public string SessionId { get; set; }
        public string ReplyToSessionId { get; set; }
        public int DeliveryCount { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime LockedUntilUtc { get; set; }
        public Guid LockToken { get; set; }
        public string MessageId { get; set; }
        public string ContentType { get; set; }
        public string PartitionKey { get; set; }
        public string ViaPartitionKey { get; set; }
        public string Label { get; set; }
        public Properties Properties { get; set; }
        public string ReplyTo { get; set; }
        public DateTime EnqueuedTimeUtc { get; set; }
        public DateTime ScheduledEnqueueTimeUtc { get; set; }
        public long SequenceNumber { get; set; }
        public long EnqueuedSequenceNumber { get; set; }
        public long Size { get; set; }
        public int State { get; set; }
        public string TimeToLive { get; set; }
        public string To { get; set; }
        public bool IsBodyConsumed { get; set; }
        public bool ForcePersistence { get; set; }
    }

    public class Properties
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
