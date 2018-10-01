using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaScript
{
    public static class Utility
    {
        public class ReplaceReader : TextReader
        {
            string from, to;
            TextReader reader;

            public ReplaceReader(string from, string to, TextReader reader)
            {
                this.from = from;
                this.to = to;
                this.reader = reader;
            }

            public bool Exist
            {
                get
                {
                    return this.reader.Peek() >= 0;
                }
            }

            public override string ReadLine()
            {
                var line = this.reader.ReadLine();
                return line.Replace(this.from, this.to);
            }

            public override void Close()
            {
                this.reader.Close();
            }

            public override string ReadToEnd()
            {
                return this.reader.ReadToEnd().Replace(this.from, this.to);
            }

            public override async Task<string> ReadLineAsync()
            {
                var line = await this.reader.ReadLineAsync();
                return line.Replace(this.from, this.to);
            }

            public override async Task<string> ReadToEndAsync()
            {
                var ret = await this.reader.ReadToEndAsync();
                return ret.Replace(this.from, this.to);
            }
        }

        public static string ToStringLite(this ParsingTypes.ScopedName name)
        {
            if (name is ParsingTypes.ScopedName.Scope scope)
            {
                return $"{scope.Item1.ToStringLite()}::{scope.Item2}";
            }
            else if (name is ParsingTypes.ScopedName.Atomic atomic)
            {
                return atomic.Item;
            }
            return "";
        }

        public static ArgumentException MakeException(string msg)
        {
            return new ArgumentException(msg);
        }

        public class ArrayEnumerator<T>
        {
            List<T> ts;
            int index;

            public ArrayEnumerator(List<T> ts)
            {
                this.ts = ts;
                this.index = 0;
            }

            public T Current
            {
                get
                {
                    return this.ts[this.index];
                }
            }

            public T GetNext(out bool isend)
            {
                var ret = this.Current;
                MoveNext(out isend);
                return ret;
            }

            public void MoveNext(out bool isend)
            {
                ++this.index;
                isend = this.index == this.ts.Count;
            }

            public void MovePrev(out bool isend)
            {
                --this.index;
                isend = false;
            }

            public void Reset()
            {
                this.index = 0;
            }
        }

        public static ArrayEnumerator<T> GetArrayEnumerator<T>(this List<T> ts)
        {
            return new ArrayEnumerator<T>(ts);
        }
    }
}
