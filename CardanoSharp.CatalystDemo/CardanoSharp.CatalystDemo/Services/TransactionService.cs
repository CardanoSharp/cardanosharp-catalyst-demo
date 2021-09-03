using CardanoSharp.CatalystDemo.Services;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.TransactionBuilding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(TransactionService))]
namespace CardanoSharp.CatalystDemo.Services
{
    public interface ITransactionService
    {
        Task<SendResponse> Send(SendRequest request, KeyPair keyPair);
    }

    public class SendRequest
    {
        public string SenderAddress { get; set; }
        public string RecieverAddress { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }

    }

    public class SendResponse
    {
        public string TransactionHash { get; set; }
    }

    public class TransactionService: ITransactionService
    {
        private readonly IBlockfrostService _blockfrostService;
        private readonly IWalletStore _walletStore;

        public TransactionService()
        {
            _blockfrostService = DependencyService.Get<IBlockfrostService>();
            _walletStore = DependencyService.Get<IWalletStore>();
        }

        public async Task<SendResponse> Send(SendRequest request, KeyPair keyPair)
        {
            //start the transaction builders
            var bodyBuilder = TransactionBodyBuilder.Create;
            var witnessBuilder = TransactionWitnessSetBuilder.Create;
            var metadataBuilder = AuxiliaryDataBuilder.Create;
            var transactionBuilder = TransactionBuilder.Create;

            //get utxos
            var utxos = await _blockfrostService.GetUtxos(request.SenderAddress);

            //build inputs
            (List<TransactionInput> inputs, decimal totalSending) = GetInputs(utxos, request.Amount);

            //add inputs to transaction
            foreach(var input in inputs)
            {
                bodyBuilder.AddInput(input.TransactionId, input.TransactionIndex);
            }

            //add outputs to transaction
            Address reciever = new Address(request.RecieverAddress);
            Address sender = new Address(request.SenderAddress);
            uint adaAmount = (uint)(request.Amount * 1000000);
            uint change = (uint)(totalSending * 1000000);

            TransactionOutput changeOutput = new TransactionOutput()
            {
                Address = sender.GetBytes(),
                Value = new TransactionOutputValue()
                {
                    Coin = change
                }
            };

            bodyBuilder.AddOutput(reciever.GetBytes(), adaAmount);
            bodyBuilder.AddOutput(changeOutput.Address, changeOutput.Value.Coin);

            //get latest slot
            var slot = await _blockfrostService.GetLatestSlot();

            //add initial fee and ttl
            uint ttl = (uint)slot + 1000;
            bodyBuilder.SetFee(0);
            bodyBuilder.SetTtl(ttl);

            //set witnesses for signing
            witnessBuilder.AddVKeyWitness(keyPair.PublicKey, keyPair.PrivateKey);

            //set metadata
            metadataBuilder.AddMetadata(1337, new { message = request.Message });

            //construct transaction
            Transaction transaction = transactionBuilder
                .SetBody(bodyBuilder)
                .SetWitnesses(witnessBuilder)
                .SetAuxData(metadataBuilder)
                .Build();

            //calculate fee
            var feeParams = await _blockfrostService.GetFeeParameters();
            var fee = transaction.CalculateFee(feeParams.MinFeeA, feeParams.MinFeeB);

            //update body and rebuild
            bodyBuilder.SetFee(fee);
            changeOutput.Value.Coin = changeOutput.Value.Coin - fee;
            transaction = transactionBuilder.Build();

            //serialize the transaction
            var signedTx = transaction.Serialize();

            var txHash = await _blockfrostService.SubmitTx(signedTx);

            return new SendResponse()
            {
                TransactionHash = txHash
            };
        }

        private (List<TransactionInput>, decimal) GetInputs(List<Models.Utxo> utxos, decimal sendAmount)
        {
            var inputs = new List<TransactionInput>();
            decimal totalSending = 0;
            foreach (var utxo in utxos)
            {
                var lovelaces = utxo.Amount.FirstOrDefault(x => x.Unit == "lovelace")?.Quantity;
                if (!lovelaces.HasValue) continue;


                var ada = lovelaces.Value / 1000000;

                if (totalSending < sendAmount)
                {
                    totalSending = totalSending + ada;
                    inputs.Add(new TransactionInput()
                    {
                        TransactionIndex = (uint)utxo.TxId,
                        TransactionId = utxo.TxHash.HexToByteArray()
                    });
                }
                else
                {
                    break;
                }
            }
            totalSending = totalSending - sendAmount;

            return (inputs, totalSending);
        }
    }
}
