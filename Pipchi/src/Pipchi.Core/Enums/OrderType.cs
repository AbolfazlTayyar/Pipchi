using System.ComponentModel.DataAnnotations;

namespace Pipchi.Core.Enums;

public enum TradeType
{
    [Display(Name = "خرید")]
    Buy = 1,

    [Display(Name = "فروش")]
    Sell = 2
}
