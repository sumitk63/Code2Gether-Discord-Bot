using System.Collections.Generic;
using System.Threading.Tasks;

namespace Code2Gether_Discord_Bot.Library.Models.Repositories
{
    public interface IDataRepository<TModel>
    {
        Task CreateAsync(TModel newModel);
        Task<TModel> ReadAsync(int id);
        Task<IEnumerable<TModel>> ReadAllAsync();
        Task UpdateAsync(TModel existingModel);
        Task DeleteAsync(int id);
    }
}