using EasyNetQ;
using EasyNetQ.Internals;
using Hotel.Common.Messaging;
using Hotel.Common.Messaging.Events;

namespace Hotel.IntegrationTests.Helpers;

public class MessageBusFake : IMessageBus
{
  public void Dispose() { }

  public Task PublishAsync<T>(T message, CancellationToken token = default) where T : BaseEventMessage
  {
    Console.WriteLine($"The Message will not be evaluated, due to integrations tests");
    return Task.CompletedTask;
  }

  public AwaitableDisposable<SubscriptionResult> SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage, CancellationToken token = default) where T : class
  {
    var queue = "any-queue";
    if (!string.IsNullOrWhiteSpace(subscriptionId)) queue = subscriptionId;
    Console.WriteLine($"Queue {subscriptionId}, will not be evaluated, due to integrations tests");
    return new AwaitableDisposable<SubscriptionResult>(
            Task.FromResult(
                new SubscriptionResult(EasyNetQ.Topology.Exchange.Default,
                  new EasyNetQ.Topology.Queue(queue),
                  DisposableAction.Create(_ => { }, 42)
                )
            )
        );
  }
}