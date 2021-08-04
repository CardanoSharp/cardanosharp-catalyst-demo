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
    public static class GenerateWallet
    {
        public class GenerateWalletCommand: IRequest<GenerateWalletResponse>
        {
            public int Size { get; private set; }

            public GenerateWalletCommand(int size)            
            {
                Size = size;
            }
        }

        public class GenerateWalletHandler : IRequestHandler<GenerateWalletCommand, GenerateWalletResponse>
        {
            private IKeyService _keyService;
            private IAddressService _addressService;

            public GenerateWalletHandler(IKeyService keyService, IAddressService addressService)
            {
                _keyService = keyService;
                _addressService = addressService;
            }

            public async Task<GenerateWalletResponse> Handle(GenerateWalletCommand request, CancellationToken cancellationToken)
            {
                var mnemonic = _keyService.Generate(request.Size);

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

                return new GenerateWalletResponse()
                {
                    Mnemonic = mnemonic,
                    Key1 = key1.PrivateKey,
                    Key2 = key2.PrivateKey,
                    Address1 = address1,
                    Address2 = address2
                };
            }
        }

        public class GenerateWalletResponse
        {
            public Mnemonic Mnemonic { get; set; }
            public PrivateKey Key1 { get; set; }
            public PrivateKey Key2 { get; set; }
            public Address Address1 { get; set; }
            public Address Address2 { get; set; }
        }
    }
}
