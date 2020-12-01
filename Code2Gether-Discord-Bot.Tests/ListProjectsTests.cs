﻿using System.Threading.Tasks;
using Code2Gether_Discord_Bot.Library.BusinessLogic;
using Code2Gether_Discord_Bot.Library.Models.Repositories.ProjectRepository;
using Code2Gether_Discord_Bot.Static;
using Code2Gether_Discord_Bot.Tests.Fakes;
using NUnit.Framework;

namespace Code2Gether_Discord_Bot.Tests
{
    internal class ListProjectsTests
    {
        private IBusinessLogic _logic;
        private IProjectRepository _repo;

        [SetUp]
        public void Setup()
        {
            var user = TestConfig.User();

            var client = new FakeDiscordClient()
            {
                FakeApplication = new FakeApplication()
                {
                    Owner = user
                }
            };

            var guild = new FakeGuild()
            {

            };

            var messageChannel = new FakeMessageChannel()
            {

            };

            var message = new FakeUserMessage()
            {
                Author = user
            };

            _repo = TestConfig.ProjectRepository();

            _logic = new ListProjectsLogic(UtilityFactory.GetLogger(GetType()), new FakeCommandContext()
            {
                Channel = messageChannel,
                Client = client,
                Guild = guild,
                Message = message,
                User = user
            }, _repo);
        }

        [Test]
        public void InstantiationTest() =>
            Assert.IsTrue(_logic != null);

        [Test]
        public async Task ExecutionTest()
        {
            _ = await _logic.ExecuteAsync();
            Assert.IsTrue(_repo.ReadAll().Count > 0);
        }

        [Test]
        public async Task EmbedExecutionTest()
        {
            var embed = await _logic.ExecuteAsync();
            Assert.IsTrue(embed.Description.Contains(_repo.Read(0).ToString()));
        }
    }
}
