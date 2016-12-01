using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace LZW_Coding
{
    class UnArhieve
    {
        Dictionary<string, int> table;
        FileStream writer;
        BinaryReader inputStream;
        string currentString = null;
        string prevString = null;

        int sizeOfInt;
        int position;
        uint currentValue;
        uint prevValue;

        public UnArhieve(Dictionary<string, int> table)
        {
            inputStream = new BinaryReader(File.OpenRead(Environment.CurrentDirectory + @"\coded.bin"));
            sizeOfInt = sizeof(int) * 8;
            this.table = table;
            position = (int)Math.Log(table.Count, 2) + 1;
        }
        private void WriteString(string str)
        {
            foreach (byte item in Encoding.Default.GetBytes(str))
                writer.WriteByte(item);
        }
        private void PresentInDict()
        {
            currentString = table.ElementAt((int)currentValue).Key;
            WriteString(currentString);
            table.Add(prevString + currentString[0], table.Count);

            prevString = currentString;
            prevValue = currentValue;
        }
        private void NotPresentInDict()
        {
            string p = table.ElementAt((int)prevValue).Key;
            prevString = p + p[0];
            WriteString(prevString);

            table.Add(prevString, table.Count);
            prevValue = (uint)table.Count - 1;
        }
        public void DeArhieve(string inputPath)
        {
            writer = new FileStream(inputPath, FileMode.Create, FileAccess.Write);

            int posInFile = 0;
            int sizeOfFile = ((int)inputStream.BaseStream.Length - 1) / 4;
            byte zerous = inputStream.ReadByte();
            uint readedValue = inputStream.ReadUInt32(); posInFile++;
            int length = (int)Math.Log(table.Count, 2) + 1;


            currentValue += (uint)(readedValue >> (sizeOfInt - position));
            currentString = table.ElementAt((int)currentValue).Key;
            WriteString(currentString);

            prevValue = currentValue;
            prevString = currentString;
            while (true)
            {
                length = (int)Math.Log(table.Count + 2, 2) + 1;
                currentValue = (uint)(readedValue << (position)) >> (sizeOfInt - length);
                while (position + length < sizeOfInt)
                {
                    if (currentValue < table.Count)
                        PresentInDict();
                    else
                        NotPresentInDict();
                    length = (int)Math.Log(table.Count + 2, 2) + 1;

                    currentValue = (uint)(readedValue << (position + (int)Math.Log(table.Count + 1, 2) + 1)) >> (sizeOfInt - length);
                    position += length;
                }
                position = (int)Math.Log(table.Count + 1, 2) + 1 - sizeOfInt + position;

                readedValue = inputStream.ReadUInt32(); posInFile++;
                if (position > 0)
                    currentValue += (uint)(readedValue >> (sizeOfInt - position));

                if (currentValue < table.Count)
                    PresentInDict();
                else
                    NotPresentInDict();

                if (sizeOfFile - posInFile == 0)
                    break;
            }
            while (position + length < sizeOfInt - zerous + 2)
            {
                length = (int)Math.Log(table.Count + 2, 2) + 1;

                currentValue = (uint)(readedValue << (position)) >> (sizeOfInt - length);
                position += length;
                if (currentValue < table.Count)
                    PresentInDict();
                else
                    NotPresentInDict();
            }
            inputStream.Close();
            writer.Close();
        }

    }
}
