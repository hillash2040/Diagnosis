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
            sim.temp("777.txt", "777_iscas85.txt", "777_Real.txt");
        }
    }
}
