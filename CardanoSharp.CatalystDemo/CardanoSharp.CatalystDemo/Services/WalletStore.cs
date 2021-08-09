using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Derivations;
using CardanoSharp.Wallet.Models.Keys;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.CatalystDemo.Services
{
    public interface IWalletStore
    {
        IAccountNodeDerivation AccountKeys { get; }

        void SetAccountKeys(Mnemonic mnemonic);
    }

    public class WalletStore : IWalletStore
    {
        public IAccountNodeDerivation AccountKeys { get; private set; }

        public void SetAccountKeys(Mnemonic mnemonic)
        {
            AccountKeys = mnemonic.GetMasterNode()
                .Derive(Wallet.Enums.PurposeType.Shelley)
                .Derive(Wallet.Enums.CoinType.Ada)
                .Derive(0);
        }
    }
}
