using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Extensions.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.CatalystDemo.Models;
using Microsoft.Extensions.Configuration;
using Flurl.Http;
using CardanoSharp.Wallet.Models.Transactions;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using CardanoSharp.Wallet.Encoding;

namespace CardanoSharp.CatalystDemo.Commands
{
    public static class SendAda
    {
        public class SendAdaCommand: IRequest<SendAdaResponse>
        {
            public (PrivateKey, PublicKey, Address) FromKeys { get; private set; }
            public string ToAddress { get; private set; }
            public decimal Amount { get; private set; }

            public SendAdaCommand((PrivateKey, PublicKey, Address) fromKeys, string toAddress, decimal amount)            
            {
                FromKeys = fromKeys;
                ToAddress = toAddress;
                Amount = amount;
            }
        }

        public class SendAdaHandler : IRequestHandler<SendAdaCommand, SendAdaResponse>
        {
            private readonly string blockfrostApiKey;
            private readonly TransactionBuilder _transactionBuilder;
            private IKeyService _keyService;
            private IAddressService _addressService;

            public SendAdaHandler(IKeyService keyService, IAddressService addressService, IConfiguration configuration)
            {
                blockfrostApiKey = configuration["Blockfrost:ApiKey"];
                _keyService = keyService;
                _addressService = addressService;
                _transactionBuilder = new TransactionBuilder();
            }

            public async Task<SendAdaResponse> Handle(SendAdaCommand request, CancellationToken cancellationToken)
            {
                List<Utxo> utxos = new List<Utxo>();
                try
                {
                    utxos = await $"https://cardano-testnet.blockfrost.io/api/v0/addresses/{request.FromKeys.Item3.ToString()}/utxos"
                        .WithHeader("project_id", blockfrostApiKey)
                        .GetJsonAsync<List<Utxo>>();
                }
                catch (Exception) { }

                //build inputs
                var inputs = new List<TransactionInput>();
                decimal totalSending = 0;
                foreach(var utxo in utxos)
                {
                    var lovelaces = utxo.Amount.FirstOrDefault(x => x.Unit == "lovelace")?.Quantity;
                    if (!lovelaces.HasValue) continue;


                    var ada = lovelaces.Value / 1000000;

                    if (totalSending < request.Amount)
                    {
                        totalSending = totalSending + ada;
                        inputs.Add(new TransactionInput()
                        {
                            TransactionIndex = (uint)utxo.TxId,
                            TransactionId = utxo.TxHash.HexToByteArray()
                        });
                    }else
                    {
                        break;
                    }
                }
                totalSending = totalSending - request.Amount;

                //build outputs
                var outputs = new List<TransactionOutput>()
                {
                    new TransactionOutput()
                    {
                        Address = Bech32.Decode(request.ToAddress, out _, out _),
                        Value = new TransactionOutputValue()
                        {
                            Coin = (uint)(request.Amount * 1000000)
                        }
                    },
                    new TransactionOutput()
                    {
                        Address = request.FromKeys.Item3.GetBytes(),
                        Value = new TransactionOutputValue()
                        {
                            Coin = (uint)(totalSending * 1000000)
                        }
                    }
                };

                var transactionBody = new TransactionBody()
                {
                    TransactionInputs = inputs,
                    TransactionOutputs = outputs,
                    Fee = 0,
                    Ttl = 33814067
                };

                var witnesses = new TransactionWitnessSet()
                {
                    VKeyWitnesses = new List<VKeyWitness>()
                    {
                        new VKeyWitness()
                        {
                            VKey = request.FromKeys.Item2.Key,
                            SKey = request.FromKeys.Item1.Key
                        }
                    }
                };

                var transaction = new Transaction()
                {
                    TransactionBody = transactionBody,
                    TransactionWitnessSet = witnesses
                };

                var draftTx = _transactionBuilder.SerializeTransaction(transaction);
                var newFee = calculateFee(draftTx);

                transactionBody.TransactionOutputs.Last().Value.Coin -= (uint)newFee;
                transactionBody.Fee = (uint)newFee;
                Console.WriteLine($"Fee: {newFee}");
                var signedTx = _transactionBuilder.SerializeTransaction(transaction);

                Console.WriteLine($"Signed Tx: {signedTx.ToStringHex()}");
                string transactionResult = string.Empty;
                try
                {
                    var url = "https://cardano-testnet.blockfrost.io/api/v0/tx/submit";
                    var httpClient = new HttpClient();
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
                    httpRequest.Content = new ByteArrayContent(signedTx);
                    httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/cbor");
                    httpRequest.Content.Headers.Add("project_id", "VLvo7vf4Xyv07DhymwK0ss7qWjiO2DKw");
                    var response = httpClient.SendAsync(httpRequest).Result;

                    transactionResult = response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
                Console.WriteLine($"Tx ID: {transactionResult}");
                return new SendAdaResponse()
                {
                    TransactionId = transactionResult
                };
            }

            private uint calculateFee(byte[] txSigned)
            {
                var fee = (txSigned.ToStringHex().Length * 44) + 155381;
                return (uint)((fee < 155381) ? 155381 : fee);
            }
        }

        public class SendAdaResponse
        {
            public string TransactionId { get; set; }
        }
    }
}
