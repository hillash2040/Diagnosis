using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    abstract class Gate
    {
        public int Id { get; protected set; }
        public int order { get; set; }
        public double P { get; set; }
        public double Cost { get; set; }
        public enum Type {and, or, xor, nor, nand, buffer, not, cone}
        protected Type type;
        private Wire output;
        public virtual Wire Output {
            get
            {
                return output;
            }
            set 
            {
                output = value;
                output.InputComponent = this; 
            } 
        }

        public Gate()
        {
            P = 0.01;
            Cost = 5;
        }
        public Gate(double cost, double p)
        {
            this.Cost = cost;
            this.P = p;
        }
        public virtual bool GetValue() { return Output.Value; }

        /*virtual - cone */
        public virtual void SetValue() // give the output wire the value that it should have
        {
            Output.Value = GetValue();
        }
        public static int CompareComponents(Gate x, Gate y)
        {
            if (x == null || y == null)
                return 0;
            if (x.order == y.order)
                return 0;
            if (x.order > y.order)
                return 1;
            else
                return -1;
        }
    }
}
