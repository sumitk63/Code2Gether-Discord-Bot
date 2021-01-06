using System;
using System.Linq;
using System.Threading.Tasks;
using Code2Gether_Discord_Bot.Library.CustomExceptions;
using Code2Gether_Discord_Bot.Library.Models.Repositories;
using Serilog;

namespace Code2Gether_Discord_Bot.Library.Models
{
    public class ProjectManager : IProjectManager
    {
        private IMemberRepository _memberRepository;
        private IProjectRepository _projectRepository;

        public ProjectManager(IMemberRepository memberRepository, IProjectRepository projectRepository)
        {
            _memberRepository = memberRepository;
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// Checks if a project exists by a given <see cref="projectName"/>.
        /// </summary>
        /// <param name="projectName">Project name to check for</param>
        /// <returns>true if the project exists. false if the project does not exist.</returns>
        public async Task<bool> DoesProjectExistAsync(string projectName)
        {
            try
            {
                await _projectRepository.ReadAsync(projectName);
                return true;
            }
            catch (DataAccessLayerTransactionFailedException e)
            {
                return false;
            }
        }

        /// <summary>
        /// Get a project by a given <see cref="projectName"/>
        /// </summary>
        /// <param name="projectName">Project name to search for</param>
        /// <returns>a project if it is found or otherwise returns null</returns>
        public Task<Project> GetProjectAsync(string projectName)
        {
            return _projectRepository.ReadAsync(projectName);
        }

        /// <summary>
        /// Creates a new project with a given name and an given author.
        /// The author is automatically added to the project.
        /// </summary>
        /// <param name="projectName">Name for new project</param>
        /// <param name="author">Member that is requesting project be made</param>
        /// <returns>A new project instance 
        /// or throws an exception if project was not created
        /// or author failed to join the project.</returns>
        public async Task<Project> CreateProjectAsync(string projectName, Member author)
        {
            try
            {
                var retrievedAuthor = await _memberRepository.ReadFromSnowflakeAsync(author.SnowflakeId);
                author = retrievedAuthor;
            }
            catch (DataAccessLayerTransactionFailedException e)
            {
                await _memberRepository.CreateAsync(author);
                author = await _memberRepository.ReadFromSnowflakeAsync(author.SnowflakeId); // Update author
            }

            var newProject = new Project(projectName, author);

            await _projectRepository.CreateAsync(newProject);

            // Retrieve project to add member to.
            newProject = await _projectRepository.ReadAsync(newProject.Name);
            await _projectRepository.AddMemberAsync(newProject, author);

            // Retrieve project with added member.
            newProject = await _projectRepository.ReadAsync(newProject.Name);

            return newProject;
        }

        /// <summary>
        /// Attempt to join a project by a given name with a given member.
        /// </summary>
        /// <param name="projectName">Project name to join</param>
        /// <param name="member">Member to join a project</param>
        public async Task JoinProjectAsync(string projectName, Member member)
        {
            try
            {
                var retrievedMember = await _memberRepository.ReadFromSnowflakeAsync(member.SnowflakeId);
                member = retrievedMember;
            }
            catch (DataAccessLayerTransactionFailedException e)
            {
                await _memberRepository.CreateAsync(member);
                member = await _memberRepository.ReadFromSnowflakeAsync(member.SnowflakeId);
            }

            // Get project matching projectName
            var project = await _projectRepository.ReadAsync(projectName);

            // If the given member by SnowflakeId does not exist in the project as a member
            // Add the member to the project.
            if (!project.Members.Any(m => m.SnowflakeId == member.SnowflakeId))
            {
                await _projectRepository.AddMemberAsync(project, member);
            }
        }
    }
}
