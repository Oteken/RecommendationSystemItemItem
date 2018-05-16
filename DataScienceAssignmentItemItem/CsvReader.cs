using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataScienceAssignmentItemItem
{
    class CsvReader
    {
        public List<string> retrieveCsvContent(FileStream fileStream)
        {
            List<string> content = new List<string>();
            StreamReader streamReader = new StreamReader(fileStream);
            while (streamReader.Peek() != -1)
            {
                string nextContent = retrieveNext(streamReader);
                if (nextContent.Length > 0)
                {
                    content.Add(nextContent);
                }
            }
            streamReader.Close();
            return content;
        }

        private string retrieveNext(StreamReader streamReader)
        {
            string fullText = "";
            int byteValue = streamReader.Read();
            char character = (char)byteValue;
            while ((byteValue > 47 && byteValue < 58) || (byteValue == 46))
            {
                fullText += character;
                byteValue = streamReader.Read();
                character = (char)byteValue;
            }
            return fullText;
        }

    }
}
