using System;
using System.Collections.Generic;

namespace otherServices.Models;

public partial class SavedPost
{
    public long TenantId { get; set; }

    public long PostId { get; set; }

    public DateTime? DateSaved { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User Tenant { get; set; } = null!;
}
