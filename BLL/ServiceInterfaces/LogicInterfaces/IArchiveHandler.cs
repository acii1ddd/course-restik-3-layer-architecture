using BLL.DTO;

namespace BLL.ServiceInterfaces.LogicInterfaces
{
    public interface IArchiveHandler
    {
        void ArchiveOrderWithItems(OrderDTO order);
    }
}
