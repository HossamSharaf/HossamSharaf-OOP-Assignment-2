using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp5
{
    public class Solution
    {
        public int MaxVowels(string s, int k)
        {
            int currentVowels = 0;

            for (int i = 0; i < k; i++)
            {
                if (IsVowel(s[i]))
                {
                    currentVowels++;
                }
            }

            int maxVowels = currentVowels;

            for (int i = k; i < s.Length; i++)
            {
                if (IsVowel(s[i]))
                {
                    currentVowels++;
                }
                if (IsVowel(s[i - k]))
                {
                    currentVowels--;
                }

                if (currentVowels > maxVowels)
                {
                    maxVowels = currentVowels;
                }


                if (maxVowels == k)
                {
                    return k;
                }
            }

            return maxVowels;
        }

        private bool IsVowel(char c)
        {
            return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u';
        }
    }
}
