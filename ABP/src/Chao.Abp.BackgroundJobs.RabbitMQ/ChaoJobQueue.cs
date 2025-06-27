using Chao.Abp.BackgroundJobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.RabbitMQ;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.MultiTenancy;
using Volo.Abp.RabbitMQ;
using Volo.Abp.Security.Claims;

namespace Chao.Abp.BackgroundJobs.RabbitMQ;

public class ChaoJobQueue<TArgs>(IOptions<AbpBackgroundJobOptions> backgroundJobOptions, IOptions<AbpRabbitMqBackgroundJobOptions> rabbitMqAbpBackgroundJobOptions, IChannelPool channelPool, IRabbitMqSerializer serializer, IBackgroundJobExecuter jobExecuter, IServiceScopeFactory serviceScopeFactory, IExceptionNotifier exceptionNotifier, IOptions<ChaoAbpBackgroundJobOption> chaoAbpBackgroundJobOptions, ChaoBackgroundEventArgBuilder chaoBackgroundEventArgBuilder) : JobQueue<TArgs>(backgroundJobOptions, rabbitMqAbpBackgroundJobOptions, channelPool, serializer, jobExecuter, serviceScopeFactory, exceptionNotifier)
{
    protected override async Task MessageReceived(object sender, BasicDeliverEventArgs ea)
    {
        var arg = Serializer.Deserialize<ChaoBackgroundEventArg<TArgs>>(ea.Body.ToArray());
        if (arg.Chao == false)
        {
            await base.MessageReceived(sender, ea);
            return;
        }
        using var scope = ServiceScopeFactory.CreateScope();
        var context = new JobExecutionContext(
            scope.ServiceProvider,
            JobConfiguration.JobType,
            arg.Arg!
        );

        try
        {
            var currentTenant = scope.ServiceProvider.GetRequiredService<ICurrentTenant>();
            var currentPrincipalAccessor = scope.ServiceProvider.GetRequiredService<ICurrentPrincipalAccessor>();
            using (currentTenant.Change(arg.TenantId, arg.TenantName))
            {
                var claims = arg.Claims.Select(c => new Claim(c.Key, c.Value)).ToArray();
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
                using (currentPrincipalAccessor.Change(claimsPrincipal))
                {
                    await JobExecuter.ExecuteAsync(context);
                }
            }
            ChannelAccessor!.Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
        catch (BackgroundJobExecutionException)
        {
            //TODO: Reject like that?
            ChannelAccessor!.Channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: true);
        }
        catch (Exception)
        {
            //TODO: Reject like that?
            ChannelAccessor!.Channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
        }
    }

    protected override async Task PublishAsync(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
    {
        if (chaoAbpBackgroundJobOptions.Value.BasicInfoEnable == false)
        {
            await base.PublishAsync(args, priority, delay);
            return;
        }
        var routingKey = QueueConfiguration.QueueName;
        var basicProperties = CreateBasicPropertiesToPublish();

        if (delay.HasValue)
        {
            routingKey = QueueConfiguration.DelayedQueueName;
            basicProperties.Expiration = delay.Value.TotalMilliseconds.ToString();
        }

        var newArg = chaoBackgroundEventArgBuilder.Configure(args);

        ChannelAccessor!.Channel.BasicPublish(
            exchange: "",
            routingKey: routingKey,
            basicProperties: basicProperties,
            body: Serializer.Serialize(newArg)
        );
    }
}