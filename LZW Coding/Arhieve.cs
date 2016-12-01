using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LZW_Coding
{
    delegate char GetChar(int byteIndex);
    class ArhieveE
    {
        Dictionary<string, int> table;
        public event GetChar getChar;
        string pathCoded;
        public ArhieveE(Dictionary<string, int> table)
        {
            this.table = table;
            this.pathCoded =Environment.CurrentDirectory + @"\coded.bin";
        }

        public void Arhieve(FileStream inputStream)
        {
            BinaryWriter outputStream = new BinaryWriter(new FileStream(pathCoded, FileMode.Create, FileAccess.Write));
            outputStream.Seek(1, SeekOrigin.Begin);

            byte sizeOfInt = sizeof(int) * 8;
            string prev = null;
            uint buffer = 0;
            int bufferLenth = 0;
            int number = 0;
            int index = 0, length = 0;
            string together = null;
            while (true)
            {
                char next;
                try { next = getChar((int)inputStream.ReadByte()); }
                catch (Exception) { break; }

                together = prev + next.ToString();

                if (table.TryGetValue(together, out number))
                    prev = together;
                else
                {
                    table.Add(together, table.Count);
                    index = table[prev];
                    length = (int)Math.Log(table.Count, 2) + 1;

                   
                    if (bufferLenth + length <= sizeOfInt)
                    {
                        buffer += (uint)(index << (sizeOfInt - bufferLenth - length));
                        bufferLenth += length;
                    }
                    else
                    {
                        bufferLenth = bufferLenth + length - sizeOfInt;
                        buffer += (uint)(index >> bufferLenth);
                        outputStream.Write(buffer);

                        buffer = (uint)(index << sizeOfInt - bufferLenth);
                    }
                    prev = next.ToString();
                }
            }
            index = table[together];
            length = (int)Math.Log(table.Count, 2) + 1;

            if (bufferLenth + length <= sizeOfInt)
            {
                buffer += (uint)(index << (sizeOfInt - bufferLenth - length));
                bufferLenth += length;
            }
            else
            {
                bufferLenth = bufferLenth + length - sizeOfInt;
                buffer += (uint)(index >> bufferLenth);
                outputStream.Write(buffer);
                buffer = (uint)(index << sizeOfInt - bufferLenth);
            }
            outputStream.Write(buffer);

            outputStream.Seek(0, SeekOrigin.Begin);
            outputStream.Write((byte)(sizeOfInt - bufferLenth));

            outputStream.Close();
            inputStream.Close();
        }
    }
}
