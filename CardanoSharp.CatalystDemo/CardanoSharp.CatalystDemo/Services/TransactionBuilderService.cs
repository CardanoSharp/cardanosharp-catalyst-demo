using Blockfrost.Api.Models;
using Blockfrost.Api.Services;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.TransactionBuilding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockfrost.Api.Models.Extensions;

namespace CardanoSharp.CatalystDemo.Services
{

    public class TransactionBuilderService : ITransactionBuilderService
    {
        private readonly ICardanoService _cardano;

        //private readonly IBlockfrostService _blockfrostService;
        private readonly IWalletStore _walletStore;
        private readonly IToast _toast;

        public TransactionBuilderService(ICardanoService cardanoService, IWalletStore walletStore, IToast toast)
        {
            _cardano = cardanoService;
            
            //_blockfrostService = DependencyService.Get<IBlockfrostService>();
            _walletStore = walletStore;
            _toast = toast;
        }

        public async Task<Transaction> BuildAsync(SendRequest request, KeyPair keyPair)
        {
            //start the transaction builders
            var bodyBuilder = TransactionBodyBuilder.Create;
            var witnessBuilder = TransactionWitnessSetBuilder.Create;
            var metadataBuilder = AuxiliaryDataBuilder.Create;
            var transactionBuilder = Wallet.TransactionBuilding.TransactionBuilder.Create;

            //get utxos
            var maxpages = 10;
            var remaining = request.Amount;
            decimal totalSending = 0;

            // We might need to fetch more than one page of utxos
            for (int cnt = 1; cnt <= maxpages; cnt++)
            {
                if (remaining < 0)
                {
                    break;
                }

                var utxos = await _cardano.Addresses.GetUtxosAsync(request.SenderAddress, page: cnt);
                remaining -= utxos.SumAmounts();

                //build inputs
                (List<TransactionInput> inputs, decimal partialSending) = GetInputs(utxos, request.Amount);
                totalSending += partialSending;

                //add inputs to transaction
                foreach (var input in inputs)
                {
                    bodyBuilder.AddInput(input.TransactionId, input.TransactionIndex);
                }
            }

            if (remaining > 0)
            {
                _toast.LongAlert("Not enough ada. Allow to fetch more UTxOs...");
                return null;
            }


            //add outputs to transaction
            Address reciever = new Address(request.RecieverAddress);
            Address sender = new Address(request.SenderAddress);
            uint adaAmount = (uint)(request.Amount * 1000000);

            bodyBuilder.AddOutput(reciever.GetBytes(), adaAmount);
            bodyBuilder.AddOutput(sender.GetBytes(), (uint)totalSending);

            //get latest slot
            var slot = (await _cardano.Blocks.GetLatestAsync()).Slot;

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
            var feeParams = await _cardano.Epochs.GetLatestParametersAsync();
            var fee = transaction.CalculateFee((uint)feeParams.MinFeeA, (uint)feeParams.MinFeeB);

            //update body and rebuild
            bodyBuilder.SetFee(fee);
            transaction = transactionBuilder.Build();
            transaction.TransactionBody.TransactionOutputs.Last().Value.Coin -= fee;

            //serialize the transaction
            return transaction;
        }

        private (List<TransactionInput>, decimal) GetInputs(AddressUtxoContentResponseCollection utxos, decimal sendAmount)
        {
            var inputs = new List<TransactionInput>();
            decimal totalSending = 0;
            sendAmount = sendAmount * 1000000;
            foreach (var utxo in utxos)
            {
                var lovelaces = utxo.SumAmounts("lovelace");
                
                if (lovelaces == 0) continue;

                if (totalSending < sendAmount)
                {
                    totalSending = totalSending + lovelaces;
                    inputs.Add(new TransactionInput()
                    {
                        TransactionIndex = (uint)utxo.TxIndex,
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
