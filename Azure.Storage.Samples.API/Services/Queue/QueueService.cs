using Azure.Storage.Queues;

namespace Azure.Storage.Samples.API.Services.Queue;

public class QueueService : IQueueService
{
    private readonly QueueServiceClient _queueServiceClient;
    private readonly ILogger<QueueService> _logger;

    public async Task CreateQueueIfNotExits(string queueName)
    {
        var queueClient = _queueServiceClient.GetQueueClient(queueName);

        await queueClient.CreateIfNotExistsAsync();
        _logger.LogInformation("queue has been created");
    }

    public async Task<bool> DeleteQueueIfExists(string queueName)
    {
        var queueClient = _queueServiceClient.GetQueueClient(queueName);

        try
        {
            await queueClient.DeleteIfExistsAsync();
            return true;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogInformation(ex, "");
            return false;

        }
    }



    public async Task<IEnumerable<string>> ReceiveMessagesAsync(string queueName, int messageCount)
    {
        var queueClient = _queueServiceClient.GetQueueClient(queueName);
        var receivedMessage = await queueClient.ReceiveMessagesAsync(messageCount);

        foreach (var message in receivedMessage.Value)
        {
            try
            {
                _logger.LogInformation(message.Body.ToString());
                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "");
                continue;

            }

        }

        return receivedMessage.Value.Select(x => x.Body.ToString());

    }

    public async Task<IEnumerable<string>> PeekMessagesAsync(string queueName, int messageCount)
    {
        var queueClient = _queueServiceClient.GetQueueClient(queueName);
        var receivedMessage = await queueClient.PeekMessagesAsync(messageCount);

        foreach (var message in receivedMessage.Value)
        {
            _logger.LogInformation(message.Body.ToString());

        }

        return receivedMessage.Value.Select(x => x.Body.ToString());


    }

    public async Task<bool> UpdateMessageAsync(string queueName, string messageId, string popReceipt, string messageText)
    {
        var queueClient = GetQueueClient(queueName);

        if (await queueClient.ExistsAsync())
            return false;

        await queueClient.UpdateMessageAsync(messageId, popReceipt, messageText);
        _logger.LogInformation("message has been updated");

        return true;

    
    }


    public QueueClient GetQueueClient(string queueName)
    {
        return _queueServiceClient.GetQueueClient(queueName);
    }

    public async  Task<bool> QueueIsExists(string queueName)
        => await GetQueueClient(queueName).ExistsAsync();


}
