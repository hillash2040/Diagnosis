using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulator sim = new Simulator();
            sim.CheckPropogation("74182.txt", "74182_iscas85.txt");
            sim.CheckPropogation("74283.txt", "74283_iscas85.txt");
            sim.CheckPropogation("74181.txt", "74181_iscas85.txt");
            sim.CheckPropogation("c432.txt", "c432_iscas85.txt");
            sim.CheckPropogation("c880.txt", "c880_iscas85.txt");
        }
    }
}
