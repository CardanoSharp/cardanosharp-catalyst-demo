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
            public GetUtxosQuery(string address)
            {
                Address = address;
            }

            public string Address { get; private set; }
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
                var url = $"https://cardano-testnet.blockfrost.io/api/v0/addresses/{request.Address}/utxos";

                var utxos = await url
                    .WithHeader("project_id", blockfrostApiKey)
                    .GetJsonAsync<List<Utxo>>();

                return new GetUtxosResponse()
                {
                    Utxos = utxos
                };
            }
        }

        public class GetUtxosResponse
        {
            public List<Utxo> Utxos { get; set; }
        }
    }
}
