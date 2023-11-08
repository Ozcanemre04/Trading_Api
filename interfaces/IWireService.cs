using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trading_app.dto.Wire;

namespace trading_app.interfaces
{
    public interface IWireService
    {
        Task<WireDto> CreateWire(AddWireDto addWireDto);
        Task<decimal> CurrentBalance();
    }
}