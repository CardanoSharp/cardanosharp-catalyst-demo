using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using System.Threading.Tasks;

namespace CardanoSharp.CatalystDemo.Services
{
    public interface ITransactionBuilderService
    {
        Task<Transaction> BuildAsync(SendRequest request, KeyPair keyPair);
    }
}
