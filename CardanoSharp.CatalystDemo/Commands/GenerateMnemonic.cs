using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Keys;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardanoSharp.CatalystDemo.Commands
{
    public static class GenerateMnemonic
    {
        public class GenerateMnemonicCommand: IRequest<GenerateMnemonicResponse>
        {
            public int Size { get; private set; }

            public GenerateMnemonicCommand(int size)            
            {
                Size = size;
            }
        }

        public class GenerateMnemonicHandler : IRequestHandler<GenerateMnemonicCommand, GenerateMnemonicResponse>
        {
            private IKeyService _keyService;

            public GenerateMnemonicHandler(IKeyService keyService)
            {
                _keyService = keyService;
            }

            public async Task<GenerateMnemonicResponse> Handle(GenerateMnemonicCommand request, CancellationToken cancellationToken)
            {
                return new GenerateMnemonicResponse()
                {
                    Mnemonic = _keyService.Generate(request.Size)
                };
            }
        }

        public class GenerateMnemonicResponse
        {
            public Mnemonic Mnemonic { get; set; }
        }
    }
}
