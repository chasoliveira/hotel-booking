using EasyNetQ;
using EasyNetQ.Internals;
using Polly;
using RabbitMQ.Client.Exceptions;
using EasyNetQ.DI;
using System.Buffers;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using Hotel.Common.Messaging.Events;

namespace Hotel.Common.Messaging;

public interface IMessageBus : IDisposable
{
  AwaitableDisposable<SubscriptionResult> SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage, CancellationToken token = default) where T : class;
  Task PublishAsync<T>(T message, CancellationToken token = default) where T : BaseEventMessage;
}

[ExcludeFromCodeCoverage]
public class MessageBus : IMessageBus
{
  private IBus _bus;
  private IAdvancedBus _advancedBus;
  private readonly string? _connectionString;
  public MessageBus(IConfiguration configuration)
  {
    _connectionString = configuration.GetConnectionString("MessageBus");
    HotelException.ThrowIfNullOrEmpty(_connectionString);
    TryConnect();
  }
  public bool IsConnected => _bus?.Advanced.IsConnected ?? false;
  
  public AwaitableDisposable<SubscriptionResult> SubscribeAsync<T>(
    string subscriptionId,
    Func<T, Task> onMessage,
    CancellationToken token = default) where T : class
  {
    TryConnect();
    return _bus.PubSub.SubscribeAsync(subscriptionId, onMessage);
  }

  public async Task PublishAsync<T>(T message, CancellationToken token = default) where T : BaseEventMessage
  {
    TryConnect();
    await _bus.PubSub.PublishAsync(message, token);
  }

  private void TryConnect()
  {
    if (IsConnected) return;

    var policy = Policy.Handle<EasyNetQException>()
        .Or<BrokerUnreachableException>()
        .RetryForever();

    policy.Execute(() =>
    {
      _bus = RabbitHutch.CreateBus(_connectionString, services =>
      {
        services.EnableConsoleLogger();
        services.EnableLegacyTypeNaming();
        services.Register<ISerializer, SystemTextJsonSerializer>();
      });
      _advancedBus = _bus.Advanced;
      _advancedBus.Disconnected += (_, _) => TryConnect();
    });
  }

  public void Dispose()
  {
    _bus.Dispose();
  }
}

#region Local Implementation until EasyNeq publish a new version
public sealed class SystemTextJsonSerializer : ISerializer
{
  private readonly System.Text.Json.JsonSerializerOptions serialiseOptions;
  private readonly System.Text.Json.JsonSerializerOptions deserializeOptions;

  public SystemTextJsonSerializer()
      : this(new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.General))
  {
  }

  public SystemTextJsonSerializer(System.Text.Json.JsonSerializerOptions options)
  {
    serialiseOptions = new System.Text.Json.JsonSerializerOptions(options);
    deserializeOptions = new System.Text.Json.JsonSerializerOptions(options);
    deserializeOptions.Converters.Add(new SystemObjectNewtonsoftCompatibleConverter());
  }

  public IMemoryOwner<byte> MessageToBytes(Type messageType, object message)
  {
    var stream = new ArrayPooledMemoryStream();
    System.Text.Json.JsonSerializer.Serialize(stream, message, messageType, serialiseOptions);
    return stream;
  }

  public object BytesToMessage(Type messageType, in ReadOnlyMemory<byte> bytes)
  {
    return System.Text.Json.JsonSerializer.Deserialize(bytes.Span, messageType, deserializeOptions)!;
  }

}
public class SystemObjectNewtonsoftCompatibleConverter : System.Text.Json.Serialization.JsonConverter<object>
{
  public override object? Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
  {
    switch (reader.TokenType)
    {
      case System.Text.Json.JsonTokenType.True:
        return true;
      case System.Text.Json.JsonTokenType.False:
        return false;
      case System.Text.Json.JsonTokenType.Number:
        return reader.TryGetInt64(out var longValue) ? longValue : reader.GetDouble();
      case System.Text.Json.JsonTokenType.String:
        return reader.TryGetDateTime(out var datetimeValue) ? datetimeValue : reader.GetString();
      default:
        {
          using var document = System.Text.Json.JsonDocument.ParseValue(ref reader);
          return document.RootElement.Clone();
        }
    }
  }

  public override void Write(System.Text.Json.Utf8JsonWriter writer, object value, System.Text.Json.JsonSerializerOptions options)
  {
    throw new InvalidOperationException("Should not get here.");
  }
}

#endregion