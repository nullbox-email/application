using System;
using System.Collections.Generic;
using System.Text;

namespace Nullbox.Fabric.Application.Common.Partitioning;

public interface IPartitionKeyScope
{
    string? Current { get; }
    IDisposable Push(string partitionKey);
}
