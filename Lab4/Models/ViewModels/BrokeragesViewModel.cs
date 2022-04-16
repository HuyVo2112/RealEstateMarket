
namespace Assignment2.Models.ViewModels
{
    public class BrokeragesViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
        public IEnumerable<Brokerage> Brokerages { get; set; }
    }    
}
