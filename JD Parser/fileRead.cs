using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace fileRead
{
    class fileRead
    {

        // read file from input path
        public static StringBuilder ReadFileToString(object path)
        {
            StringBuilder text = new StringBuilder();
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object miss = System.Reflection.Missing.Value;
            object readOnly = true;
            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);

            for (int i = 0; i < docs.Paragraphs.Count; i++)
            {
                text.Append(Regex.Replace(docs.Paragraphs[i + 1].Range.Text.ToString(), @"[^\w\d\s\-\/\,\.\(\#\:\)\+\$\'\!\@]", "") + "\n");
            }
            return text;
        }


     
    }
}
