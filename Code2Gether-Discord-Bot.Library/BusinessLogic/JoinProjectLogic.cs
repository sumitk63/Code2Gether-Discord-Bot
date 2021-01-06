using System;
using System.Linq;
using System.Threading.Tasks;
using Code2Gether_Discord_Bot.Library.CustomExceptions;
using Code2Gether_Discord_Bot.Library.Models;
using Code2Gether_Discord_Bot.Library.Static;
using Discord;
using Discord.Commands;
using Serilog;

namespace Code2Gether_Discord_Bot.Library.BusinessLogic
{
    public class JoinProjectLogic : BaseLogic
    {
        private IProjectManager _projectManager;
        private string _arguments;

        public JoinProjectLogic(ILogger logger, ICommandContext context, IProjectManager projectManager, string arguments) : base(logger, context)
        {
            _projectManager = projectManager;
            _arguments = arguments;
        }

        public override async Task<Embed> ExecuteAsync()
        {
            var embedContent = await JoinProjectAsync();

            var embed = new EmbedBuilder()
                .WithColor(Color.Purple)
                .WithTitle($"Join Project: {embedContent.Title}")
                .WithDescription(embedContent.Description)
                .WithAuthor(_context.User)
                .Build();

            return embed;
        }

        private async Task<EmbedContent> JoinProjectAsync()
        {
            var embedContent = new EmbedContent();

            var projectName = ParseCommandArguments.ParseBy(' ', _arguments)[0];

            var user = new Member(_context.User);

            Project project = null;

            // Attempt to join the project
            try
            {
                await _projectManager.JoinProjectAsync(projectName, user);
                project = await _projectManager.GetProjectAsync(projectName);

                embedContent.Title = "Success";
                embedContent.Description = $"{_context.User} has successfully joined project **{projectName}**!"
                                           + Environment.NewLine
                                           + Environment.NewLine
                                           + project;

                // If project has become active from new user
                if (project.IsActive)
                    TransitionToActiveProject(project);
            }
            catch (DataAccessLayerTransactionFailedException e)
            {
                embedContent.Title = "Failed";
                embedContent.Description = $"{_context.User} failed to join project **{projectName}**!"
                                           + Environment.NewLine
                                           + Environment.NewLine
                                           + project;
            }
            
            return embedContent;
        }

        private async void TransitionToActiveProject(Project project)
        {
            // Find a category in the guild called "PROJECTS"
            ulong? projCategoryId = _context.Guild
                .GetCategoriesAsync().Result
                .FirstOrDefault(c => c.Name
                    .Contains("PROJECTS"))?.Id;

            // Create new text channel under that category
            bool channelAlreadyCreated = false;
            var channels = await _context.Guild.GetChannelsAsync();
            ITextChannel channel = null;
            if (channels.Count(c => c.Name.Contains(project.Name)) == 0)
            {
                channel = await _context.Guild.CreateTextChannelAsync(project.Name, p =>
                {
                    if (projCategoryId != null)
                        p.CategoryId = projCategoryId;
                });
            }
            else
            {
                channelAlreadyCreated = true;
            }

            // Create new role
            var roleName = $"project-{project.Name}";
            var roles = _context.Guild.Roles;
            IRole role;
            if (roles.Count(r => r.Name.Contains(roleName)) == 0)
            {
                role = await _context.Guild
                    .CreateRoleAsync(roleName, GuildPermissions.None, null, false, true);
            }
            else
            {
                role = _context.Guild.Roles.FirstOrDefault(r => r.Name.Contains(roleName));
            }

            // Give every project member the role
            foreach (var member in project.Members)
            {
                await _context.Guild
                    .GetUserAsync(member.SnowflakeId).Result
                    .AddRoleAsync(role);
            }

            if (!channelAlreadyCreated)
            {
                // Notify members in new channel
                await channel.SendMessageAsync(embed: new EmbedBuilder()
                    .WithColor(Color.Purple)
                    .WithTitle("New Active Project")
                    .WithDescription($"A new project has gained enough members to become active!"
                                     + Environment.NewLine
                                     + project)
                    .Build());
            }
        }
    }
}
