using RabbitMQ.Client;
using System.Text;
namespace Queue1.AppQueue.Functions
{
    public class RabbitMQAdmin : IMessageQueue
    {
        private readonly string _hostName = "localhost";
        private readonly string _userName = "guest";
        private readonly string _password = "guest";

        public string HostName => _hostName;
        public string UserName => _userName;
        public string Password => _password;

        public void EnqueueMessage(string message)
        {
            SendMessageToQueue("emailQueue", message);
        }

        public void SendMessageToQueue(string queueName, string message)
        {
            SendMessagesToQueue(queueName, new List<string> { message });
        }

        public void EnqueueMessages(IEnumerable<string> messages)
        {
            SendMessagesToQueue("emailQueue", messages);
        }

        public void SendMessagesToQueue(string queueName, IEnumerable<string> messages)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = HostName,
                    UserName = UserName,
                    Password = Password
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                foreach (var message in messages)
                {
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        basicProperties: null,
                        body: body);

                    Console.WriteLine($"Message sent to queue {queueName}: {message}");
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, queueName);
            }
        }

        private void HandleException(Exception ex, string queueName)
        {
            switch (ex)
            {
                case RabbitMQ.Client.Exceptions.BrokerUnreachableException brokerEx:
                    Console.WriteLine($"Error: Unable to connect to RabbitMQ server. {brokerEx.Message}");
                    break;
                case RabbitMQ.Client.Exceptions.OperationInterruptedException opEx:
                    Console.WriteLine($"Error: Operation was interrupted. {opEx.Message}");
                    break;
                default:
                    Console.WriteLine($"Error sending message(s) to queue {queueName}: {ex.Message}");
                    break;
            }
        }
    }
}
