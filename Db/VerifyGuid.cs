using System;
using System.Collections.Generic;

namespace TaskFour.Db;

public partial class VerifyGuid
{
    public int UserId { get; set; }

    public string Guid { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
