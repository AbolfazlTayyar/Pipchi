using System;
using System.Collections.Generic;
using System.Text;

namespace Pipchi.Core.Exceptions;

public class InvalidVolumeRangeException : Exception
{
    public InvalidVolumeRangeException() : base("The specified volume range is invalid.")
    {
        
    }
}
