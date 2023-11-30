using VMS.SharedKernel;
using System.Threading.Tasks;

namespace VMS.Interface
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch(BaseDomainEvent domainEvent);
    }
}
