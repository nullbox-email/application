using Nullbox.Fabric.Application.Common.Partitioning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nullbox.Fabric.Infrastructure.Partitioning;

public sealed class PartitionKeyScope : IPartitionKeyScope
{
    private readonly Stack<string> _stack = new();

    public string? Current => _stack.Count > 0 ? _stack.Peek() : null;

    public IDisposable Push(string partitionKey)
    {
        _stack.Push(partitionKey);
        return new Popper(_stack);
    }

    private sealed class Popper : IDisposable
    {
        private Stack<string>? _stack;
        public Popper(Stack<string> stack) => _stack = stack;
        public void Dispose()
        {
            if (_stack is null) return;
            _stack.Pop();
            _stack = null;
        }
    }
}
