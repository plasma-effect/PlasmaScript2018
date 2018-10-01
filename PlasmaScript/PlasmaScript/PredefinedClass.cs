using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaScript
{
    namespace Predefined
    {
        public class BasicValue<T>
            where T:struct
        {
            public BasicValue(T value = default(T))
            {
                this.Value = value;
            }

            void Assign(T val)
            {
                this.Value = val;
            }

            T Value { get; set; }
        }
        public class BITInt
        {
            long[] ar;

            public BITInt(int length)
            {
                this.ar = new long[length];
            }

            public long this[int index]
            {
                get
                {
                    return this.ar[index];
                }
                set
                {
                    this.ar[index] = value;
                }
            }
            public int Length
            {
                get
                {
                    return this.ar.Length;
                }
            }
        }
        public class BITDouble
        {
            double[] ar;

            public BITDouble(int length)
            {
                this.ar = new double[length];
            }

            public double this[int index]
            {
                get
                {
                    return this.ar[index];
                }
                set
                {
                    this.ar[index] = value;
                }
            }
            public int Length
            {
                get
                {
                    return this.ar.Length;
                }
            }
        }
        public class SegTree<T>
        {
            T[] ar;
            
            public Func<T, T, T> Monoid { get; }

            public SegTree(int depth, Func<T, T, T> monoid)
            {
                this.ar = new T[(1 << this.Depth) - 1];
                this.Depth = depth;
                this.Monoid = monoid;
            }

            public T this[int index]
            {
                get
                {
                    return this.ar[index];
                }
                set
                {
                    this.ar[index] = value;
                }
            }

            public int Depth { get; }

            public int Length
            {
                get
                {
                    return this.ar.Length;
                }
            }
        }
        public class ModNum
        {
            static ulong mod = 1000000007;
            
            public ulong Value { get; }

            public ModNum(ulong val)
            {
                this.Value = val % mod;
            }

            public static ModNum operator +(ModNum lhs, ModNum rhs)
            {
                return new ModNum(lhs.Value + rhs.Value);
            }

            public static ModNum operator -(ModNum lhs, ModNum rhs)
            {
                return new ModNum(lhs.Value + mod - rhs.Value);
            }

            public static ModNum operator *(ModNum lhs, ModNum rhs)
            {
                return new ModNum(lhs.Value * rhs.Value);
            }

            public static ModNum operator /(ModNum lhs, ModNum rhs)
            {
                return lhs * Inverse(rhs);
            }

            private static ModNum Inverse(ModNum val)
            {
                return Pow(val, mod - 2);
            }

            public static ModNum Pow(ModNum val, ulong count)
            {
                return count == 0 ? new ModNum(1) :
                    count == 1 ? val :
                    count == 2 ? val * val :
                    count % 2 == 0 ?
                    Pow(Pow(val, count / 2), 2) :
                    Pow(Pow(val, count / 2), 2) * val;
            }
        }
        public class DualArray<T>
        {
            T[,] ar;

            public DualArray(T[,] ar, int dim0, int dim1)
            {
                this.ar = ar;
                this.Dim0 = dim0;
                this.Dim1 = dim1;
            }

            public int Dim0 { get; }
            public int Dim1 { get; }

            public class SubArray
            {
                DualArray<T> dual;
                int index;

                public SubArray(DualArray<T> dual, int index)
                {
                    this.dual = dual;
                    this.index = index;
                }

                public T this[int index]
                {
                    get
                    {
                        return this.dual.ar[this.index, index];
                    }
                    set
                    {
                        this.dual.ar[this.index, index] = value;
                    }
                }

            }
            public SubArray this[int index]
            {
                get
                {
                    return new SubArray(this, index);
                }
            }
        }
        public class PriorityQueue<T>
            where T : IComparable
        {
            List<T> set;

            public PriorityQueue()
            {
                this.set = new List<T>();
            }

            public void Push(T val)
            {
                this.set.Add(val);
                foreach(var i in Enumerable.Range(0, this.set.Count - 1).Reverse())
                {
                    if (this.set[i].CompareTo(this.set[i + 1]) < 0)
                    {
                        var t = this.set[i];
                        this.set[i] = this.set[i + 1];
                        this.set[i + 1] = t;
                    }
                }
            }

            public T Top
            {
                get
                {
                    return this.set.Last();
                }
            }

            public void Pop()
            {
                this.set.RemoveAt(this.set.Count - 1);
            }

            public int Count
            {
                get
                {
                    return this.set.Count;
                }
            }
        }
        public class VoidType
        {
            public VoidType()
            {

            }
        }
    }
}