using System;
using System.Collections.Generic;

namespace DotnetCore.Core.Interface
{
    public interface ICacheable
    {
        CacheOption CacheOption { get; }
        KeyValuePair<string, TimeSpan> CacheSettings { get; }
    }

    public interface INoCache
    {
        bool NoCache { get; set; }
    }

    public enum CacheOption
    {
        None = 0,
        Memory = 1,
        Distributed = 2,
        Multi = 3
    }
}