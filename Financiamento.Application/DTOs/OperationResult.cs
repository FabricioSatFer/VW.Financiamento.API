using System;
using System.Collections.Generic;

namespace Financiamento.Application.DTOs
{
    public class OperationResult<T>
    {
        public bool Success { get; init; }
        public T? Value { get; init; }
        public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
        public string? Code { get; init; }
    }
}
