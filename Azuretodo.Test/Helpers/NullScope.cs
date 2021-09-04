using System;

namespace Azuretodo.Test.Helpers
{
    public class NullScope : IDisposable
    {
        public static NullScope Isntance { get; } = new NullScope(); 
        public void Dispose() { }
        private NullScope() { }
    }
}
