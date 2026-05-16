using JDownloader.Model;
using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class Events(ComicsJDownloaderClient client) :
        BaseNamespace(client), IEvents
    {
        public override string Endpoint => "events";

        public Task<Subscription> Addsubscription(long subscriptionId, string[] subscriptions, string[] exclusions)
        {
            return PostRequestAsync<Subscription>("addsubscription", new object[3] { subscriptionId, subscriptions, exclusions });
        }

        public Task<Subscription> ChangeSubscriptionTimeouts(long subscriptionId, long pollTimeout, long maxKeepAlive)
        {
            return PostRequestAsync<Subscription>("changesubscriptiontimeouts", new object[3] { subscriptionId, pollTimeout, maxKeepAlive });
        }

        public Task<Subscription> GetSubscription(long subscriptionId)
        {
            return PostRequestAsync<Subscription>("getsubscription", new object[1] { subscriptionId });
        }

        public Task<SubscriptionStatus> GetSubscriptionStatus(long subscriptionId)
        {
            return PostRequestAsync<SubscriptionStatus>("getsubscriptionstatus", new object[1] { subscriptionId });
        }

        public Task Listen(long subscriptionId)
        {
            return PostRequestAsync<SubscriptionStatus>("listen", new object[1] { subscriptionId });
        }

        public Task<Publisher> ListPublisher()
        {
            return PostRequestAsync<Publisher>("listpublisher");
        }

        public Task<Subscription> RemoveSubscription(long subscriptionId, string[] subscriptions, string[] exclusions)
        {
            return PostRequestAsync<Subscription>("removesubscription", new object[3] { subscriptionId, subscriptions, exclusions });
        }

        public Task<Subscription> SetSubscription(long subscriptionId, string[] subscriptions, string[] exclusions)
        {
            return PostRequestAsync<Subscription>("setsubscription", new object[3] { subscriptionId, subscriptions, exclusions });
        }

        public Task<Subscription> Subscribe(string[] subscriptions, string[] exclusions)
        {
            return PostRequestAsync<Subscription>("subscribe", new object[2] { subscriptions, exclusions });
        }

        public Task<Subscription> Unsubscribe(long subscriptionId)
        {
            return PostRequestAsync<Subscription>("unsubscribe", new object[1] { subscriptionId });
        }
    }
}