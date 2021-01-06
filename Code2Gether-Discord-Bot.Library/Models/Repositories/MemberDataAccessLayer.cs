using System.Threading.Tasks;
using Code2Gether_Discord_Bot.Library.CustomExceptions;
using Newtonsoft.Json;
using RestSharp;

namespace Code2Gether_Discord_Bot.Library.Models.Repositories
{
    public class MemberDataAccessLayer : WebApiDataAccessLayerBase<Member>, IMemberRepository
    {
        protected override string _tableRoute => "Members";

        public MemberDataAccessLayer(string connectionString) : base(connectionString) { }
        
        protected override string SerializeModel(Member memberToSerialize)
        {
            var newMember = new Member { SnowflakeId = memberToSerialize.SnowflakeId };

            var json = JsonConvert.SerializeObject(newMember);

            return json;
        }

        /// <summary>
        /// Finds a member with the snowflake ID.
        /// </summary>
        /// <param name="snowflakeId">Snowflake ID of member to find.</param>
        /// <returns>Member with snowflake ID. Null if no record found.</returns>
        public async Task<Member> ReadFromSnowflakeAsync(ulong snowflakeId)
        {
            var request = new RestRequest($"{_tableRoute}/snowflakeID={snowflakeId}");

            var result = await GetClient().ExecuteGetAsync<Member>(request);

            if (!result.IsSuccessful) throw new DataAccessLayerTransactionFailedException($"Read via snowflake ID {snowflakeId} failed!");

            return result.Data;
        }
    }
}
