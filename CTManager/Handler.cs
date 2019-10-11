using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTManager
{
    class Handler
    {
        public class Options
        {
            public bool isSingleLine; // 单行结果
            public bool isRN; // 使用 \r\n 而不是 \n
            public bool isNIgnored; // 忽略换行
            public bool isBackslashIgnored; // 忽略转义符
            public bool isFormatIgnored; // 忽略格式说明符
            public bool isQuoteIgnored; // 忽略引号
            public override string ToString() 
            {
                var ret = new StringBuilder();
                ret.Append(isSingleLine ? '1' : '0');
                ret.Append(isRN ? '1' : '0');
                ret.Append(isNIgnored ? '1' : '0');
                ret.Append(isBackslashIgnored ? '1' : '0');
                ret.Append(isFormatIgnored ? '1' : '0');
                ret.Append(isQuoteIgnored ? '1' : '0');
                return ret.ToString();
            }
            public Options() { }
            public Options(string str)
            {
                isSingleLine = (str[0] == '1');
                isRN = (str[1] == '1');
                isNIgnored = (str[2] == '1');
                isBackslashIgnored = (str[3] == '1');
                isFormatIgnored = (str[4] == '1');
                isQuoteIgnored = (str[5] == '1');
            }
            public Options Clone()
            {
                return new Options(ToString());
            }
        }

        Options options;

        public Handler() { }
        public Handler(Options o) => options = o;

        private void append(ref StringBuilder b, params char[] ch)
        {
            foreach (var c in ch)
                b.Append(c);
        }

        public String Switch(String src)
        {
            if (src.Length == 0)
                return src;

            StringBuilder ret = new StringBuilder();

            bool isPre = false; // 用于标记是否有过换行

            if (!options.isQuoteIgnored)
                append(ref ret, '"');
            foreach (var ch in src)
            {
                if (ch == '\r') // 都是 \r
                {
                    if (!options.isNIgnored)
                    {
                        if (options.isRN) // 如果是 \r\n
                            append(ref ret, '\\', 'r');
                        append(ref ret, '\\', 'n');
                    }
                    isPre = true;
                }
                else // 如果不是换行符
                {
                    if (!options.isSingleLine && isPre) // 如果不是换行符，并且上一个符号为换行符，并且开启了多行模式
                    {
                        if (!options.isQuoteIgnored)
                            append(ref ret, '"');
                        append(ref ret, '\r');
                        if (!options.isQuoteIgnored)
                            append(ref ret, '"');
                    }
                    isPre = false; // 清空换行标志

                    if (ch == '%' && !options.isFormatIgnored) // 处理格式说明符
                        append(ref ret, '%', '%');
                    else if (ch == '"' && !options.isQuoteIgnored) // 处理引号
                        append(ref ret, '\\', '\"');
                    else if (ch == '\\' && !options.isBackslashIgnored) // 处理转义符
                        append(ref ret, '\\', '\\');
                    else if (ch == '\t') //处理制表符
                        append(ref ret, '\\', 't');
                    else
                        append(ref ret, ch);
                }
            }
            if (!options.isQuoteIgnored)
                append(ref ret, '"');

            return ret.ToString();
        }
    }
}