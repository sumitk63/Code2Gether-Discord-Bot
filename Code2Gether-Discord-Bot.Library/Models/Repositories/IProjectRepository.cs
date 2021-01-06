using System.Threading.Tasks;

namespace Code2Gether_Discord_Bot.Library.Models.Repositories
{
    public interface IProjectRepository : IDataRepository<Project>
    {
        Task<Project> ReadAsync(string projectName);
        Task AddMemberAsync(Project project, Member member);
        Task RemoveMemberAsync(Project project, Member member);
    }
}
