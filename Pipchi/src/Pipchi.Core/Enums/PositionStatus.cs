using System.ComponentModel.DataAnnotations;

namespace Pipchi.Core.Enums;

public enum PositionStatus
{
    [Display(Name = "باز")]
    Open,

    [Display(Name = "بسته")]
    Closed
}
