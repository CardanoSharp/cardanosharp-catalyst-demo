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

namespace CardanoSharp.CatalystDemo.Commands
{
    public static class RestoreWallet
    {
        public class RestoreWalletCommand: IRequest<RestoreWalletResponse>
        {
            public string Words { get; private set; }

            public RestoreWalletCommand(string words)            
            {
                Words = words;
            }
        }

        public class RestoreWalletHandler : IRequestHandler<RestoreWalletCommand, RestoreWalletResponse>
        {
            private IKeyService _keyService;
            private IAddressService _addressService;

            public RestoreWalletHandler(IKeyService keyService, IAddressService addressService)
            {
                _keyService = keyService;
                _addressService = addressService;
            }

            public async Task<RestoreWalletResponse> Handle(RestoreWalletCommand request, CancellationToken cancellationToken)
            {
                var mnemonic = _keyService.Restore(request.Words);

                var account = mnemonic.GetMasterNode()
                    .Derive(PurposeType.Shelley)
                    .Derive(CoinType.Ada)
                    .Derive(0);

                var key1 = account
                    .Derive(RoleType.ExternalChain)
                    .Derive(0);
                key1.SetPublicKey();

                var key2 = account
                    .Derive(RoleType.ExternalChain)
                    .Derive(1);
                key2.SetPublicKey();

                var stake = account
                    .Derive(RoleType.Staking)
                    .Derive(0);
                stake.SetPublicKey();

                var address1 = _addressService.GetAddress(key1.PublicKey, stake.PublicKey, NetworkType.Testnet, AddressType.Base);
                var address2 = _addressService.GetAddress(key2.PublicKey, stake.PublicKey, NetworkType.Testnet, AddressType.Base);

                return new RestoreWalletResponse()
                {
                    Mnemonic = mnemonic,
                    Key1 = (key1.PrivateKey, key1.PublicKey, address1),
                    Key2 = (key2.PrivateKey, key2.PublicKey, address2)
                };
            }
        }

        public class RestoreWalletResponse
        {
            public Mnemonic Mnemonic { get; set; }
            public (PrivateKey, PublicKey, Address) Key1 { get; set; }
            public (PrivateKey, PublicKey, Address) Key2 { get; set; }
        }
    }
}
