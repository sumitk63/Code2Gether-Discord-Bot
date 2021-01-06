using System;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code2Gether_Discord_Bot.Library.CustomExceptions;
using Newtonsoft.Json;

namespace Code2Gether_Discord_Bot.Library.Models.Repositories
{
    public abstract class WebApiDataAccessLayerBase<TDataModel> : IDataRepository<TDataModel>
        where TDataModel : class, IDataModel
    {
        protected string _connectionString;
        protected abstract string _tableRoute { get; }

        protected WebApiDataAccessLayerBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new record of the input model.
        /// </summary>
        /// <param name="newModel">Model to add to DB.</param>
        /// <returns>True if successful.</returns>
        public async Task CreateAsync(TDataModel newModel)
        {
            var client = GetClient();

            var request = new RestRequest(_tableRoute);

            var jsonBody = SerializeModel(newModel);

            request.AddJsonBody(jsonBody, "application/json");

            var result = await client.ExecutePostAsync<TDataModel>(request);

            if (!result.IsSuccessful) throw new DataAccessLayerTransactionFailedException($"Creation of {newModel.GetType().Name} failed!");
        }

        protected virtual string SerializeModel(TDataModel modelToSerialize)
        {
            var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            return JsonConvert.SerializeObject(modelToSerialize, settings);
        }

        /// <summary>
        /// Get all records in the DB.
        /// </summary>
        /// <returns>All records in the DB.</returns>
        public async Task<IEnumerable<TDataModel>> ReadAllAsync()
        {
            var client = GetClient();

            var request = new RestRequest(_tableRoute);

            var result = await client.ExecuteGetAsync<IList<TDataModel>>(request);

            if (!result.IsSuccessful) throw new DataAccessLayerTransactionFailedException($"Read All of {typeof(TDataModel).Name} failed!");

            return result.Data;
        }

        /// <summary>
        /// Gets the record for the input primary key.
        /// </summary>
        /// <param name="id">Primary key of record to retrieve.</param>
        /// <returns>Record data if retrieved, null if not.</returns>
        public async Task<TDataModel> ReadAsync(int id)
        {
            var client = GetClient();

            var request = new RestRequest($"{_tableRoute}/{id}");

            var result = await client.ExecuteGetAsync<TDataModel>(request);

            if (!result.IsSuccessful) throw new DataAccessLayerTransactionFailedException($"Read of {typeof(TDataModel).Name} failed!");

            return result.Data;
        }

        /// <summary>
        /// Deletes the record with the input primary key.
        /// </summary>
        /// <param name="id">Primary key of record to delete.</param>
        /// <returns>True if delete is successful.</returns>
        public async Task DeleteAsync(int id)
        {
            var client = GetClient();

            var request = new RestRequest($"{_tableRoute}/{id}", Method.DELETE);

            var result = await client.ExecuteAsync<TDataModel>(request);

            if (!result.IsSuccessful) throw new DataAccessLayerTransactionFailedException($"Deletion at id {id} failed!");
        }

        /// <summary>
        /// Updates the selected record.
        /// </summary>
        /// <param name="modelToReplace">Record to update.</param>
        /// <returns>False if the record doesn't exist, or if the update failed.</returns>
        public async Task UpdateAsync(TDataModel modelToReplace)
        {
            var client = GetClient();

            var request = new RestRequest($"{_tableRoute}/{modelToReplace.ID}", Method.PUT);

            var jsonBody = SerializeModel(modelToReplace);

            request.AddJsonBody(jsonBody);

            var result = await client.ExecuteAsync<TDataModel>(request);

            if (!result.IsSuccessful) throw new DataAccessLayerTransactionFailedException($"Update of {typeof(TDataModel).Name} failed!");
        }

        protected RestClient GetClient()
        {
            return new RestClient(_connectionString);
        }
    }
}
