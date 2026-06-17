using System;
using System.Collections.Generic;

namespace TaskFour.Db;

public partial class SignInTimestamp
{
    public int UserId { get; set; }

    public double Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
