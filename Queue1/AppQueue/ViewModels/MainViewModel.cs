using Queue1.AppQueue.Functions;
using Queue1.AppQueue.Microservice;
using Queue1.AppQueue.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Queue1.AppQueue.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly Producer _producer;

        private string _statusMessage;
        private double _progress;
        private bool _isSending;

        public ObservableCollection<MailMessages> Messages { get; set; } = new ObservableCollection<MailMessages>();

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSending
        {
            get => _isSending;
            set
            {
                if (_isSending != value)
                {
                    _isSending = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanSendMessages)); // Actualiza el estado del botón
                }
            }
        }

        public bool CanSendMessages => !IsSending && Messages.Count > 0; // Botón habilitado si hay mensajes y no está enviando

        public ICommand SendMessagesCommand { get; }
        public ICommand SendBulkMessagesCommand { get; }

        public MainViewModel()
        {
            _producer = new Producer(new RabbitMQAdmin());
            SendMessagesCommand = new Command(async () => await SendMessagesAsync(), () => CanSendMessages);
            SendBulkMessagesCommand = new Command(SendBulkMessages, () => CanSendMessages);

            // Agregar mensajes de prueba al iniciar (opcional, puedes eliminarlos)
            Messages.Add(new MailMessages("recipient1@example.com", "Test 1", "This is a test message 1", "sender@example.com", "priority"));
            Messages.Add(new MailMessages("recipient2@example.com", "Test 2", "This is a test message 2", "sender@example.com", "priority"));
        }

        private async Task SendMessagesAsync()
        {
            if (Messages.Count == 0)
            {
                StatusMessage = "No messages to send.";
                return;
            }

            IsSending = true;
            Progress = 0;
            int totalMessages = Messages.Count;

            try
            {
                for (int i = 0; i < totalMessages; i++)
                {
                    var message = Messages[i];

                    await Task.Delay(10); // Simula el tiempo de envío
                    _producer.SendMessageToQueue(message);

                    Progress = (i + 1) / (double)totalMessages;
                    StatusMessage = $"Sent {i + 1} of {totalMessages} messages.";
                }

                StatusMessage = "All messages sent successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error while sending messages: {ex.Message}";
            }
            finally
            {
                IsSending = false;
            }
        }

        private void SendBulkMessages()
        {
            if (Messages.Count == 0)
            {
                StatusMessage = "No messages to send in bulk.";
                return;
            }

            try
            {
                _producer.SendBulkMessagesToQueue(Messages);
                StatusMessage = "All messages sent in bulk successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error sending bulk messages: {ex.Message}";
            }
        }
    }
}
