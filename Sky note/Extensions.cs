/*--------------------------------------------------------------------------------------------------------------------
 Copyright (C) 2021 Himber Sacha

 This program is free software: you can redistribute it and/or modify
 it under the +terms of the GNU General Public License as published by
 the Free Software Foundation, either version 2 of the License, or
 any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program.  If not, see https://www.gnu.org/licenses/gpl-2.0.html. 

--------------------------------------------------------------------------------------------------------------------*/

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
