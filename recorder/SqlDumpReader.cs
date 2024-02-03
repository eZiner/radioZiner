using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace radioZiner
{
    public class Table
    {
        public string Name;
        public List<Column> Columns;
        public List<Row> Rows;

        public Table(string Name)
        {
            this.Name = Name;
            Columns = new List<Column>();
            Rows = new List<Row>();
        }
    }

    public class Column
    {
        public string Name;
        public string Type;



        public Column(string Name, string Type)
        {
            this.Name = Name;
            this.Type = Type;
        }
    }

    public class Row
    {
        public string Name;
        public int Rows;



        public Row(string Name, int Rows)
        {
            this.Name = Name;
            this.Rows = Rows;
        }
    }

    public class SqlDumpReader
    {
        public static string GetTableName(string line)
        {
            // CREATE TABLE ` column_stats`  (
            try
            {
                return line.Substring(line.IndexOf("`") + 1, line.LastIndexOf("`") - (line.IndexOf("`") + 1));
            }
            catch
            {
                return "NO TABLE";
            }
        }

        public static string GetTableType(string line)
        {
            // `column_name` varchar(64)
            try
            {
                var type = Regex.Match(line, "(?<=` )\\w+(\\(\\d+\\))?");
                return type.ToString();
            }
            catch
            {
                return "NO TYPE";
            }

        }

        public static int GetTableRows(string line)
        {
            // `column_name` varchar(64)
            try
            {
                return System.Convert.ToInt32(Regex.Match(line, "(?<=AUTO_INCREMENT=)\\d+"));
            }
            catch
            {
                return 0;
            }

        }

        public static Dictionary<string, List<string>> Convert(string filepath)
        {
            List<Table> tables = new List<Table>();
            Dictionary<string, List<string>> csvTablesDump = new Dictionary<string, List<string>>();
            // CREATE TABLE `
            using (StreamReader sr = new StreamReader(filepath))
            {
                while (sr.Peek() >= 0)
                {
                    string lineFromFile = sr.ReadLine();
                    if (lineFromFile.StartsWith("CREATE TABLE"))
                    {
                        Table table = new Table(GetTableName(lineFromFile));

                        tables.Add(table);
                        lineFromFile = sr.ReadLine();
                        while (lineFromFile.TrimStart('\t').TrimStart(' ').StartsWith("`"))
                        {
                            Column column = new Column(GetTableName(lineFromFile), GetTableType(lineFromFile));
                            table.Columns.Add(column);
                            lineFromFile = sr.ReadLine();
                            if (lineFromFile.StartsWith(")"))
                            {
                                Row rows = new Row(GetTableName(lineFromFile), GetTableRows(lineFromFile));
                                table.Rows.Add(rows);
                            }
                        }
                    } if (lineFromFile.StartsWith("INSERT INTO"))
                    {
                        int pos = lineFromFile.IndexOf('`');
                        if (pos>0 && (pos+1)< lineFromFile.Length)
                        {
                            int pos2 = lineFromFile.IndexOf('`', pos + 1);
                            if (pos2>0)
                            {
                                string sTableName = lineFromFile.Substring(pos + 1, pos2 - pos - 1);
                                if (!csvTablesDump.Keys.Contains(sTableName))
                                {
                                    csvTablesDump.Add(sTableName, new List<string>());
                                }
                                pos = lineFromFile.IndexOf('(', pos2 + 1);
                                if (pos > 0 && (pos + 1) < lineFromFile.Length)
                                {
                                    pos2 = lineFromFile.LastIndexOf(')');
                                    if (pos2 > 0)
                                    {
                                        // "),(" is cheap hack
                                        var a = lineFromFile.Substring(pos + 1, pos2 - pos - 1).Split(new string[] { "),(" }, StringSplitOptions.None);
                                        foreach (var s in a)
                                        {
                                            //Console.WriteLine(":" + s);
                                            csvTablesDump[sTableName].Add(s);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Console.WriteLine(tables.Count);
            foreach (var tbl in tables)
            {
                /*
                Console.WriteLine("> TblName: {0} (Columns: {1})", tbl.Name, tbl.Columns.Count);
                foreach (var colmn in tbl.Columns)
                {
                    Console.WriteLine("\t\t | ColName: {2} - Type: {3}", tbl.Name, tbl.Columns.Count, colmn.Name, colmn.Type);
                }
                foreach (var rows in tbl.Rows)
                {
                    Console.WriteLine("=== {0} | Count: {1}", tbl.Name, rows.Rows);
                }
                */
            }

            return csvTablesDump;
        }
    }
}