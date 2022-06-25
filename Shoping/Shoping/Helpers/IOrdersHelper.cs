using Shoping.Common;
using Shoping.Models;

namespace Shoping.Helpers
{
    public interface IOrdersHelper
    {
        Task<Response> ProcessOrderAsync(ShowCartViewModel model);
        
        Task<Response> CancelOrderAsync(int id);
    }
}
