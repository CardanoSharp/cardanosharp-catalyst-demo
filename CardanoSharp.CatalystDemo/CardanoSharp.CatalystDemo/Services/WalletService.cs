using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Derivations;
using CardanoSharp.Wallet.Models.Keys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CardanoSharp.CatalystDemo.Services
{
    
    public interface IWalletService
    {
        Task<Address> GetAddress(IAccountNodeDerivation accountNode, int index);
        Task<PrivateKey> GetPrivateKey(IAccountNodeDerivation accountNode, int index);
        Task<KeyPair> GetKeyPair(IAccountNodeDerivation accountNode, int index);
        Task<PublicKey> GetPublicKey(PrivateKey privateKey);
        Task<Mnemonic> GenerateMnemonic(int size);
        Task<Mnemonic> RestoreMnemonic(string words);
    }

    public class WalletService : IWalletService
    {
        private readonly IAddressService _addressService;
        private readonly IKeyService _keyService;

        public WalletService()
        {
            _addressService = DependencyService.Get<IAddressService>();
            _keyService = DependencyService.Get<IKeyService>(); ;
        }

        public async Task<Mnemonic> GenerateMnemonic(int size)
        {
            var mnemonic = _keyService.Generate(size);

            return await Task.FromResult(mnemonic);
        }

        public async Task<Address> GetAddress(IAccountNodeDerivation accountNode, int index)
        {
            var paymentPub = accountNode
                .Derive(RoleType.ExternalChain)
                .Derive(index)
                .PrivateKey.GetPublicKey(false);

            var stakePub = accountNode
                .Derive(RoleType.Staking)
                .Derive(0)
                .PrivateKey.GetPublicKey(false);

            var address = _addressService.GetAddress(paymentPub, stakePub, NetworkType.Testnet, AddressType.Base);

            return await Task.FromResult(address);
        }

        public async Task<PrivateKey> GetPrivateKey(IAccountNodeDerivation accountNode, int index)
        {
            var privateKey = accountNode
                .Derive(RoleType.ExternalChain)
                .Derive(index)
                .PrivateKey;

            return await Task.FromResult(privateKey);
        }

        public async Task<KeyPair> GetKeyPair(IAccountNodeDerivation accountNode, int index)
        {
            var privateKey = accountNode
                .Derive(RoleType.ExternalChain)
                .Derive(index)
                .PrivateKey;
            var keyPair = new KeyPair(privateKey, privateKey.GetPublicKey(false));

            return await Task.FromResult(keyPair);
        }

        public async Task<PublicKey> GetPublicKey(PrivateKey privateKey)
        {
            var publicKey = privateKey.GetPublicKey(false);

            return await Task.FromResult(publicKey);
        }

        public async Task<Mnemonic> RestoreMnemonic(string words)
        {
            var mnemonic = _keyService.Restore(words);

            return await Task.FromResult(mnemonic);
        }
    }
}
