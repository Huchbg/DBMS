using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMS.Utilities
{
    public static class Utils
    {

        
        //split one separator number of split times
        public static string[] Split(string input, char separator, int count)
        {
            string[] splitInput = new string[count];
            int counter = 0;
            string tempString = string.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == separator && counter != count - 1)
                {
                    splitInput[counter] = tempString;
                    counter++;
                    tempString = string.Empty;
                    continue;
                }
                tempString += input[i];
            }
            splitInput[counter] = tempString;

            return splitInput;
        }

        //self explanatory
        public static string ToUpper(string input)
        {
            string upper = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] >= 'a' && input[i] <= 'z')
                {
                    upper += Convert.ToChar(input[i] - 32);
                    continue;
                }
                upper += input[i];
            }

            return upper;
        }

        //split multiple separators
        public static string[] Split(string input, char[] separators)
        {
            int counter = 1;

            // Count the number of substrings based on separator occurrences
            for (int i = 0; i < input.Length; i++)
                for (int k = 0; k < separators.Length; k++)
                    if (input[i] == separators[k])
                        counter++;

            string[] splitInput = new string[counter];
            counter = -1;
            string tempString = string.Empty;
            bool isSeparator;

            // Loop through each character in the input string to build split segments
            for (int i = 0; i < input.Length; i++)
            {
                isSeparator = false;

                // Check if current character is a separator
                for (int k = 0; k < separators.Length; k++)
                {
                    if (input[i] == separators[k])
                    {
                        isSeparator = true;
                        break;
                    }
                }

                // If not a separator, add to tempString; otherwise, store tempString in splitInput
                if (!isSeparator)
                {
                    tempString += input[i];
                    continue;
                }

                counter++;
                splitInput[counter] = tempString;
                tempString = string.Empty;
            }

            // Store the last segment in splitInput
            splitInput[counter + 1] = tempString;

            return splitInput;
        }

        //split only with one separator and no predefined number 
        public static string[] Split(string input, char separator)
        {
            int counter = 1;

            // Count separator occurrences to determine array size
            for (int i = 0; i < input.Length; i++)
                if (input[i] == separator)
                    counter++;

            string[] splitInput = new string[counter];
            counter = -1;
            string tempString = string.Empty;

            // Build segments between separators and store them in splitInput
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == separator)
                {
                    counter++;
                    splitInput[counter] = tempString;
                    tempString = string.Empty;
                }
                else
                {
                    tempString += input[i];
                }
            }

            // Add the final segment
            splitInput[counter + 1] = tempString;

            return splitInput;
        }


        public static string TrimStart(this string str, params char[] trimChars)
        {
            // Check if the input string is null
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str), "String cannot be null.");
            }

            // If no trim characters are specified, default to whitespace characters
            if (trimChars == null || trimChars.Length == 0)
            {
                trimChars = new char[] { ' ', '\t', '\n', '\r' };
            }

            int index = 0;

            // Loop through the string and find the first non-trim character
            while (index < str.Length)
            {
                bool isTrimChar = false;

                // Check if the current character is in trimChars
                for (int i = 0; i < trimChars.Length; i++)
                {
                    if (str[index] == trimChars[i])
                    {
                        isTrimChar = true;
                        break; // Exit the loop if a trim character is found
                    }
                }

                if (!isTrimChar)
                {
                    break; // If the character is not a trim character, break the loop
                }

                index++;
            }

            // Create a new string from the remaining characters
            char[] resultChars = new char[str.Length - index];
            int resultIndex = 0;

            for (int i = index; i < str.Length; i++)
            {
                resultChars[resultIndex] = str[i];
                resultIndex++;
            }

            return new string(resultChars);
        }

        public static string TrimEnd(this string str, params char[] trimChars)
        {
            // Check if the input string is null
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str), "String cannot be null.");
            }

            // If no trim characters are specified, default to whitespace characters
            if (trimChars == null || trimChars.Length == 0)
            {
                trimChars = new char[] { ' ', '\t', '\n', '\r' };
            }

            int index = str.Length - 1; // Start from the end of the string

            // Loop backwards through the string to find the first non-trim character
            while (index >= 0)
            {
                bool isTrimChar = false;

                // Check if the current character is in trimChars
                for (int i = 0; i < trimChars.Length; i++)
                {
                    if (str[index] == trimChars[i])
                    {
                        isTrimChar = true;
                        break; // Exit the loop if a trim character is found
                    }
                }

                if (!isTrimChar)
                {
                    break; // If the character is not a trim character, break the loop
                }

                index--; // Move to the previous character
            }

            // Create a new string from the remaining characters
            char[] resultChars = new char[index + 1];
            for (int i = 0; i <= index; i++)
            {
                resultChars[i] = str[i];
            }

            return new string(resultChars);
        }

        public static string[] Slice(string[] arr, int beginning, int ending)
        {
            string[] sliced = new string[ending - beginning];
            int k = 0;
            for (int i = beginning; i < ending; i++, k++)
            {
                sliced[k] = arr[i];
            }
            return sliced;
        }

        public static string[] Slice(string[] arr, int beginning)
        {
            string[] sliced = new string[arr.Length - beginning];
            int k = 0;
            for (int i = beginning; i < arr.Length; i++, k++)
            {
                sliced[k] = arr[i];
            }
            return sliced;
        }

        public static string Trim(string input, char[] trimChars)
        {
            int start = 0;
            int end = input.Length - 1;

            // Find the first non-trim character from the start
            while (start <= end)
            {
                bool found = false;
                for (int i = 0; i < trimChars.Length; i++)
                {
                    // check if the current character is present in the trimChars array
                    if (input[start] == trimChars[i])
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    // if it's not a trim character, then break the loop
                    break;
                }
                start++;
            }

            // Find the last non-trim character from the end
            while (end >= start)
            {
                bool found = false;
                for (int i = 0; i < trimChars.Length; i++)
                {
                    // check if the current character is present in the trimmedchars
                    if (input[end] == trimChars[i])
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    // if it's not a trim character, then break the loop
                    break;
                }
                end--;
            }

            // create a new string with the non-trim characters
            char[] modified = new char[end - start + 1];
            int k = 0;
            for (int i = start; i <= end; i++)
            {
                modified[k++] = input[i];
            }
            // create a new string from the char array
            return new string(modified);
        }
        public static string Trim(string input, char trimChar)
        {
            int start = 0;
            int end = input.Length - 1;

            // Find the first non-trim character from the start
            while (start <= end && input[start] == trimChar)
            {
                start++;
            }

            // Find the last non-trim character from the end
            while (end >= start && input[end] == trimChar)
            {
                end--;
            }

            // create a new string with the non-trim characters
            char[] modified = new char[end - start + 1];
            int k = 0;
            for (int i = start; i <= end; i++)
            {
                modified[k++] = input[i];
            }
            return new string(modified);
        }

    }
}
