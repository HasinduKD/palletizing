using System.Data;

namespace Demo
{
    public class BoxState
    {
        // Index of box in layer. Not Box ID in csv file.
        public int Index { get; set; }
        public bool IsShowing { get; set; }
        public DataRow Detail { get; set; }
    }
}