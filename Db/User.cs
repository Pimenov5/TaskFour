using System;
using System.Collections.Generic;

namespace TaskFour.Db;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Status { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<SignInTimestamp> SignInTimestamps { get; set; } = new List<SignInTimestamp>();

    public virtual VerifyGuid? VerifyGuid { get; set; }
}
