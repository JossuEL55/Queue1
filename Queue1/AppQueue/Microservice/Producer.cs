using Queue1.AppQueue.Functions;
using Queue1.AppQueue.Models;

namespace Queue1.AppQueue.Microservice
{
    public class Producer
    {
        private readonly IMessageQueue _queueManager;

        public Producer(IMessageQueue queueManager)
        {
            _queueManager = queueManager ?? throw new ArgumentNullException(nameof(queueManager));
        }

        public void SendMessageToQueue(MailMessages message)
        {
            if (message == null)
            {
                Console.WriteLine("Message cannot be null.");
                return;
            }

            try
            {
                _queueManager.EnqueueMessage(message.ToString());
                Console.WriteLine($"Message successfully sent to queue: {message.Subject}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public void SendBulkMessagesToQueue(IEnumerable<MailMessages> messages)
        {
            if (messages == null)
            {
                Console.WriteLine("Message list cannot be null.");
                return;
            }

            foreach (var message in messages)
            {
                SendMessageToQueue(message);
            }
        }
    }
}   