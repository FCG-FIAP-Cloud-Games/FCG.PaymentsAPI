using FCG.Payments.Application.Messaging;
using MassTransit;

namespace FCG.Payments.Api.Messaging;

public sealed class OrderPlacedConsumer(
    IOrderPlacedEventProcessor processor,
    ILogger<OrderPlacedConsumer> logger) : IConsumer<OrderPlacedEvent>
{
    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        try
        {
            OrderPlacedEventValidator.Validate(context.Message);
        }
        catch (InvalidOrderPlacedEventException exception)
        {
            logger.LogWarning(
                exception,
                "Invalid OrderPlacedEvent. EventId={EventId}, CorrelationId={CorrelationId}, OrderId={OrderId}",
                context.Message?.EventId,
                context.Message?.CorrelationId,
                context.Message?.OrderId);
            throw;
        }

        try
        {
            var result = await processor.ProcessAsync(context.Message, context.CancellationToken);
            logger.LogInformation(
                "OrderPlacedEvent processed. EventId={EventId}, CorrelationId={CorrelationId}, OrderId={OrderId}, PaymentId={PaymentId}, Outcome={Outcome}, Duplicate={Duplicate}",
                context.Message.EventId,
                context.Message.CorrelationId,
                context.Message.OrderId,
                result.PaymentId,
                result.Outcome,
                result.Outcome is OrderPlacedProcessingOutcome.DuplicateEvent or OrderPlacedProcessingOutcome.DuplicateOrder);
        }
        catch (Exception exception) when (exception is not InvalidOrderPlacedEventException)
        {
            logger.LogError(
                exception,
                "Failed to process OrderPlacedEvent. EventId={EventId}, CorrelationId={CorrelationId}, OrderId={OrderId}",
                context.Message.EventId,
                context.Message.CorrelationId,
                context.Message.OrderId);
            throw;
        }
    }
}
