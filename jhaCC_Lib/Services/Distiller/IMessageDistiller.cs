using jhaCC.Models;
using System.Threading.Tasks;

namespace jhaCC.Services.Distiller
{
    // interface to message distiller.  Take raw data and make turn it into something generic
    public interface IMessageDistiller
    {
        // process a raw message (caller can consume via ConsumeMessageAsync in their implementaion)
        public Task DistillMessageAsync(string rawMessage);

        // if you have a MessageDetails object, this will cause it to be accumulated by IFeedData implementation
        public Task ConsumeMessageAsync(MessageDetails messageDetails);
    }
}