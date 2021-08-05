using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using Flurl;
using Flurl.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CardanoSharp.CatalystDemo.Models;

namespace CardanoSharp.CatalystDemo.Queries
{
    public static class GetUtxos
    {
        public class GetUtxosQuery : IRequest<GetUtxosResponse>
        {
            public GetUtxosQuery(string address1, string address2)
            {
                Address1 = address1;
                Address2 = address2;
            }

            public string Address1 { get; private set; }
            public string Address2 { get; private set; }
        }

        /// <summary>
        /// Passes in the transaction id to query the blockchain to get a specfic transaction and return transaction data found in the Transaction data response.
        /// </summary>
        public class GetUtxosHandler : IRequestHandler<GetUtxosQuery, GetUtxosResponse>
        {
            private readonly string blockfrostApiKey;
            public GetUtxosHandler(IConfiguration configuration)
            {
                blockfrostApiKey = configuration["Blockfrost:ApiKey"];
            }

            public async Task<GetUtxosResponse> Handle(GetUtxosQuery request, CancellationToken cancellationToken)
            {
                List<Utxo> utxos1 = new List<Utxo>();
                List<Utxo> utxos2 = new List<Utxo>();
                try
                {
                    utxos1 = await $"https://cardano-testnet.blockfrost.io/api/v0/addresses/{request.Address1}/utxos"
                        .WithHeader("project_id", blockfrostApiKey)
                        .GetJsonAsync<List<Utxo>>();
                }
                catch (Exception) { }

                try
                {
                    utxos2 = await $"https://cardano-testnet.blockfrost.io/api/v0/addresses/{request.Address2}/utxos"
                        .WithHeader("project_id", blockfrostApiKey)
                        .GetJsonAsync<List<Utxo>>();
                }
                catch (Exception) { }

                return new GetUtxosResponse()
                {
                    Utxos1 = utxos1,
                    Utxos2 = utxos2
                };
            }
        }

        public class GetUtxosResponse
        {
            public List<Utxo> Utxos1 { get; set; }
            public List<Utxo> Utxos2 { get; set; }
        }
    }
}
