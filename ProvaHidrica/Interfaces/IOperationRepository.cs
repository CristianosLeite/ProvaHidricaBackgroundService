using System.Collections.ObjectModel;
using ProvaHidrica.Models;

namespace ProvaHidrica.Interfaces
{
    public interface IOperationRepository
    {
        Task<ObservableCollection<Operation>> LoadOperations();
        Task<List<Operation>> GetOperationsByDate(
            string opInfo,
            string initialDate,
            string finalDate
        );
    }
}
