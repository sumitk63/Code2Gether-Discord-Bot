using Code2Gether_Discord_Bot.Library.Models.Repositories;

namespace Code2Gether_Discord_Bot.Static
{
    public static class RepositoryFactory
    {
        public static IProjectRepository GetProjectRepository() =>
            new ProjectDataAccessLayer(UtilityFactory.GetConfig().ConnectionString);

        public static IMemberRepository GetMemberRepository() =>
            new MemberDataAccessLayer(UtilityFactory.GetConfig().ConnectionString);
    }
}
