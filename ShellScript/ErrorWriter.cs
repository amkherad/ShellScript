using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShellScript
{
    public class ErrorWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public TextWriter Writer { get; }

        public ErrorWriter(TextWriter writer)
        {
            Writer = writer;
        }

        private ConsoleColor _color;

        private void _beginWrite()
        {
            _color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
        }

        private void _endWrite()
        {
            Console.ForegroundColor = _color;
        }

        public override void Close()
        {
            Writer.Close();
        }

        protected override void Dispose(bool disposing)
        {
            Writer.Dispose();
        }

        public override void Flush()
        {
            Writer.Flush();
        }

        public override Task FlushAsync()
        {
            return Writer.FlushAsync();
        }

        public override IFormatProvider FormatProvider => Writer.FormatProvider;
        public override string NewLine
        {
            get => Writer.NewLine;
            set => Writer.NewLine = value;
        }
        public override object InitializeLifetimeService()
        {
            return Writer.InitializeLifetimeService();
        }

        public override bool Equals(object obj)
        {
            return Writer.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Writer.GetHashCode();
        }

        public override string ToString()
        {
            return Writer.ToString();
        }

        public override void Write(bool value)
        {
            _beginWrite();
            Writer.Write(value);
        }

        public override void Write(char value)
        {
            _beginWrite();
            Writer.Write(value);
            _endWrite();
        }

        public override void Write(char[] buffer)
        {
            _beginWrite();
            Writer.Write(buffer);
            _endWrite();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _beginWrite();
            Writer.Write(buffer, index, count);
            _endWrite();
        }

        public override void Write(decimal value)
        {
            _beginWrite();
            Writer.Write(value);
            _endWrite();
        }

        public override void Write(double value)
        {
            _beginWrite();
            Writer.Write(value);
            _endWrite();
        }

        public override void Write(int value)
        {
            _beginWrite();
            Writer.Write(value);
            _endWrite();
        }

        public override void Write(long value)
        {
            _beginWrite();
            Writer.Write(value);
            _endWrite();
        }

        public override void Write(object value)
        {
            _beginWrite();
            Writer.Write(value);
            _endWrite();
        }

        public override void Write(float value)
        {
            _beginWrite();
            Writer.Write(value);
            _endWrite();
        }

        public override void Write(string value)
        {
            _beginWrite();
            Writer.Write(value);
            _endWrite();
        }

        public override void Write(string format, object arg0)
        {
            _beginWrite();
            Writer.Write(format, arg0);
            _endWrite();
        }

        public override void Write(string format, object arg0, object arg1)
        {
            _beginWrite();
            Writer.Write(format, arg0, arg1);
            _endWrite();
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            _beginWrite();
            Writer.Write(format, arg0, arg1, arg2);
            _endWrite();
        }

        public override void Write(string format, params object[] arg)
        {
            _beginWrite();
            Writer.Write(format, arg);
            _endWrite();
        }

        public override void Write(uint value)
        {
            _beginWrite();
            Writer.Write(value);
            _endWrite();
        }

        public override void Write(ulong value)
        {
            _beginWrite();
            Writer.Write(value);
            _endWrite();
        }

        public override void Write(ReadOnlySpan<char> buffer)
        {
            _beginWrite();
            Writer.Write(buffer);
            _endWrite();
        }

        public override async Task WriteAsync(char value)
        {
            _beginWrite();
            await Writer.WriteAsync(value);
            _endWrite();
        }

        public override async Task WriteAsync(char[] buffer, int index, int count)
        {
            _beginWrite();
            await Writer.WriteAsync(buffer, index, count);
            _endWrite();
        }

        public override async Task WriteAsync(ReadOnlyMemory<char> buffer,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _beginWrite();
            await Writer.WriteAsync(buffer, cancellationToken);
            _endWrite();
        }

        public override async Task WriteAsync(string value)
        {
            _beginWrite();
            await Writer.WriteAsync(value);
            _endWrite();
        }

        public override void WriteLine()
        {
            _beginWrite();
            Writer.WriteLine();
            _endWrite();
        }

        public override void WriteLine(bool value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(char value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(char[] buffer)
        {
            _beginWrite();
            Writer.WriteLine(buffer);
            _endWrite();
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            _beginWrite();
            Writer.WriteLine(buffer, index, count);
            _endWrite();
        }

        public override void WriteLine(decimal value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(double value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(int value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(long value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(object value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(float value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(string value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(string format, object arg0)
        {
            _beginWrite();
            Writer.WriteLine(format, arg0);
            _endWrite();
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            _beginWrite();
            Writer.WriteLine(format, arg0, arg1);
            _endWrite();
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            _beginWrite();
            Writer.WriteLine(format, arg0, arg1, arg2);
            _endWrite();
        }

        public override void WriteLine(string format, params object[] arg)
        {
            _beginWrite();
            Writer.WriteLine(format, arg);
            _endWrite();
        }

        public override void WriteLine(uint value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(ulong value)
        {
            _beginWrite();
            Writer.WriteLine(value);
            _endWrite();
        }

        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            _beginWrite();
            Writer.WriteLine(buffer);
            _endWrite();
        }

        public override async Task WriteLineAsync()
        {
            _beginWrite();
            await Writer.WriteLineAsync();
            _endWrite();
        }

        public override async Task WriteLineAsync(char value)
        {
            _beginWrite();
            await Writer.WriteLineAsync(value);
            _endWrite();
        }

        public override async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            _beginWrite();
            await Writer.WriteLineAsync(buffer, index, count);
            _endWrite();
        }

        public override async Task WriteLineAsync(ReadOnlyMemory<char> buffer,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _beginWrite();
            await Writer.WriteLineAsync(buffer, cancellationToken);
            _endWrite();
        }

        public override async Task WriteLineAsync(string value)
        {
            _beginWrite();
            await Writer.WriteLineAsync(value);
            _endWrite();
        }
    }
}