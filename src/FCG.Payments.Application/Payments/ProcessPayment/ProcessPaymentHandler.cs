using FCG.Payments.Application.Abstractions.Repositories;
using FCG.Payments.Domain.Payments;

namespace FCG.Payments.Application.Payments.ProcessPayment;

public sealed class ProcessPaymentHandler(
    IPaymentRepository paymentRepository,
    IPaymentSimulationStrategy simulationStrategy)
{
    public async Task<ProcessPaymentResult?> HandleAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        if (paymentId == Guid.Empty)
        {
            throw new ArgumentException("PaymentId must not be empty.", nameof(paymentId));
        }

        var payment = await paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
        {
            return null;
        }

        var previousStatus = payment.Status;
        if (previousStatus != PaymentStatus.Pending)
        {
            return new ProcessPaymentResult(
                false,
                payment.Id,
                payment.OrderId,
                payment.CorrelationId,
                previousStatus,
                payment.Status);
        }

        var finalStatus = simulationStrategy.Decide(payment);
        payment.UpdateStatus(finalStatus);
        payment.AddAttempt(finalStatus, createdAt: payment.UpdatedAt);
        await paymentRepository.SaveChangesAsync(cancellationToken);

        return new ProcessPaymentResult(
            true,
            payment.Id,
            payment.OrderId,
            payment.CorrelationId,
            previousStatus,
            payment.Status);
    }
}
