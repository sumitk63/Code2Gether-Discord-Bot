using System;
using System.Threading.Tasks;

namespace Code2Gether_Discord_Bot.Library.Models
{
    public interface IProjectManager
    {
        Task<bool> DoesProjectExistAsync(string projectName);
        Task<Project> GetProjectAsync(string projectName);
        Task<Project> CreateProjectAsync(string projectName, Member author);
        Task JoinProjectAsync(string projectName, Member member);
    }
}
