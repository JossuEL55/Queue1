namespace Queue1.AppQueue.Functions
{
    public interface IMessageQueue
    {
        void EnqueueMessage(string message);
        void SendMessageToQueue(string queueName, string message);
        void EnqueueMessages(IEnumerable<string> messages);
        void SendMessagesToQueue(string queueName, IEnumerable<string> messages);
    }
}
