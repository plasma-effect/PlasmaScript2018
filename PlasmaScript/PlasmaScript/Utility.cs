using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaScript
{
    static class Utility
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
    }
}
