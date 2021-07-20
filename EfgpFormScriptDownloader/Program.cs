using Dapper;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace EfgpFormScriptDownloader {
    class Program {
        static void Main(string[] args) {
            if (args.Length != 3) {
                Console.WriteLine("參數必須是[1]資料庫主機位址 [2]資料庫名稱 [3]目的路徑");

                return;
            }

            using var conn = new SqlConnection($"Password=Qwer1234;Persist Security Info=True;User ID=sa;Initial Catalog={args[1]};Data Source={args[0]}");
            string querySyntax = @"SELECT FormDefinition.id ,FormDefinition.formDefinitionName ,FormCategory.formCategoryName ,FormDefinition.script
                FROM FormDefinition
                    INNER JOIN FormDefinitionCmItem ON FormDefinitionCmItem.id = FormDefinition.id
                    INNER JOIN FormCategory ON FormCategory.OID = FormDefinitionCmItem.categoryOID
                WHERE FormDefinition.publicationStatus = 'RELEASED'
                ORDER BY FormCategory.formCategoryName, FormDefinition.id";
            var queryResult = conn.Query(querySyntax).ToList();

            foreach (var result in queryResult) {
                var directoryPath = args[2] + "\\" + result.formCategoryName + "\\" + result.formDefinitionName;

                if (!Directory.Exists(directoryPath)) {
                    Directory.CreateDirectory(directoryPath);
                }

                var fileName = directoryPath + "\\" + result.id + ".js";
                File.WriteAllText(fileName, result.script);
                Console.WriteLine($"表單[{result.formDefinitionName}]匯出完成");
            }
        }
    }
}
