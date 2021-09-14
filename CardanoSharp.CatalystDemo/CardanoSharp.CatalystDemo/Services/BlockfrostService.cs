using CardanoSharp.CatalystDemo.Models;
using CardanoSharp.CatalystDemo.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CardanoSharp.CatalystDemo.Services
{
    public interface IBlockfrostService
    {
        Task<FeeParameters> GetFeeParameters();
        Task<List<Utxo>> GetUtxos(string address);
        Task<string> SubmitTx(byte[] signedTx);

        Task<int> GetLatestSlot();
    }

    public class BlockfrostService: IBlockfrostService
    {
        private readonly string _apiKey = "BLOCKFROST_API_KEY_HERE";

        public async Task<int> GetLatestSlot()
        {
            try
            {
                var url = $"https://cardano-testnet.blockfrost.io/api/v0/blocks/latest";
                using (var httpClient = new HttpClient())
                {
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                    httpRequest.Headers.Add("project_id", _apiKey);
                    var response = await httpClient.SendAsync(httpRequest);

                    Block block = JsonConvert.DeserializeObject<Block>(await response.Content.ReadAsStringAsync());
                    return block.Slot;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return -1;
            }
        }

        public async Task<FeeParameters> GetFeeParameters()
        {
            try
            {
                var url = $"https://cardano-testnet.blockfrost.io/api/v0/epochs/latest/parameters";
                using (var httpClient = new HttpClient())
                {
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                    httpRequest.Headers.Add("project_id", _apiKey);
                    var response = await httpClient.SendAsync(httpRequest);

                    FeeParameters fee = JsonConvert.DeserializeObject<FeeParameters>(await response.Content.ReadAsStringAsync());
                    return fee;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        public async Task<List<Utxo>> GetUtxos(string address)
        {
            try
            {
                var url = $"https://cardano-testnet.blockfrost.io/api/v0/addresses/{address}/utxos";
                using (var httpClient = new HttpClient())
                {
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                    httpRequest.Headers.Add("project_id", _apiKey);
                    var response = await httpClient.SendAsync(httpRequest);

                    return JsonConvert.DeserializeObject<List<Utxo>>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        public async Task<string> SubmitTx(byte[] signedTx)
        {
            try
            {
                var url = "https://cardano-testnet.blockfrost.io/api/v0/tx/submit";
                using (var httpClient = new HttpClient())
                {
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
                    httpRequest.Content = new ByteArrayContent(signedTx);
                    httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/cbor");
                    httpRequest.Content.Headers.Add("project_id", _apiKey);
                    var response = httpClient.SendAsync(httpRequest).Result;

                    var text = await response.Content.ReadAsStringAsync();
                    return text;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }
    }
}
