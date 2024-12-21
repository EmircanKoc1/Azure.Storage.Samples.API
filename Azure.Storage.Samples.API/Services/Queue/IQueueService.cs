using Azure.Storage.Queues;

namespace Azure.Storage.Samples.API.Services.Queue
{
    public interface IQueueService
    {
        Task CreateQueueIfNotExits(string queueName);
        Task<bool> DeleteQueueIfExists(string queueName);
        Task<IEnumerable<string>> ReceiveMessagesAsync(string queueName, int messageCount);
        Task<IEnumerable<string>> PeekMessagesAsync(string queueName, int messageCount);
        Task<bool> UpdateMessageAsync(string queueName, string messageId, string popReceipt, string messageText);
        QueueClient GetQueueClient(string queueName);
        Task<bool> QueueIsExists(string queueName);
    }
}
