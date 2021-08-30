using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sky_note
{
    internal static class Extensions
    {
        internal static bool TryGetString(this Encoding encoding, byte[] bytes, out string result)
        {
            return encoding.TryGetString(bytes, 0, bytes.Length, out result);
        }

        internal static bool TryGetString(this Encoding encoding, byte[] bytes, int index, int count, out string result)
        {
            result = null;
            Decoder decoder = encoding.GetDecoder();
            decoder.Fallback = DecoderFallback.ExceptionFallback;
            try
            {
                int charCount = decoder.GetCharCount(bytes, index, count);
                char[] chars = new char[charCount];
                decoder.GetChars(bytes, index, count, chars, 0);
                result = new string(chars);
                return true;
            }
            catch (DecoderFallbackException)
            {
                return false;
            }
        }
    }
}
