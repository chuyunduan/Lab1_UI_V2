using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace ClassLibrary
{
    [Serializable]
    struct Grid1D
    {
        public float Step { get; set; }
        public int Count { get; set; }

        public Grid1D(float step, int count)
        {
            Step = step;
            Count = count;
        }

        public override string ToString()
        {
            return $"Step: { Step.ToString() } ; Count: { Count.ToString() }";
        }

        public string ToString(string format)
        {
            return $"Step: { Step.ToString(format) } ; Count: { Count.ToString(format) }";
        }
    }
}
