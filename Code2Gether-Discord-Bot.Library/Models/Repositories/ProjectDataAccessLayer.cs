using System.Threading.Tasks;
using Code2Gether_Discord_Bot.Library.CustomExceptions;
using RestSharp;

namespace Code2Gether_Discord_Bot.Library.Models.Repositories
{
    public class ProjectDataAccessLayer : WebApiDataAccessLayerBase<Project>, IProjectRepository
    {
        protected override string _tableRoute => "Projects";

        public ProjectDataAccessLayer(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Retrieves project based on project name.
        /// </summary>
        /// <param name="projectName">Name of project to retrieve.</param>
        /// <returns>Data for project to retrieve. Null if not found.</returns>
        public async Task<Project> ReadAsync(string projectName)
        {
            var request = new RestRequest($"{_tableRoute}/projectName={projectName}");

            var result = await GetClient().ExecuteGetAsync<Project>(request);

            if (!result.IsSuccessful) throw new DataAccessLayerTransactionFailedException($"Read via project name {projectName} failed!");

            return result.Data;
        }

        /// <summary>
        /// Adds a member to a project.
        /// </summary>
        /// <param name="project">Project of member to add.</param>
        /// <param name="member">Member to add to project.</param>
        /// <returns>True if add is successful.</returns>
        public async Task AddMemberAsync(Project project, Member member)
        {
            var request = new RestRequest($"{_tableRoute}/projectId={project.ID};memberId={member.ID}");

            var result = await GetClient().ExecutePostAsync<Project>(request);

            if (!result.IsSuccessful) throw new DataAccessLayerTransactionFailedException($"Add member {member} to project {project} failed!");
        }

        /// <summary>
        /// Adds a member to a project.
        /// </summary>
        /// <param name="project">Project of member to delete.</param>
        /// <param name="member">Member to delete from project.</param>
        /// <returns>True if delete is successful.</returns>
        public async Task RemoveMemberAsync(Project project, Member member)
        {
            var request = new RestRequest($"{_tableRoute}/projectId={project.ID};memberId={member.ID}", Method.DELETE);

            var result = await GetClient().ExecuteAsync<Project>(request);

            if (!result.IsSuccessful) throw new DataAccessLayerTransactionFailedException($"Removal of {member} from {project} failed!");
        }
    }
}
