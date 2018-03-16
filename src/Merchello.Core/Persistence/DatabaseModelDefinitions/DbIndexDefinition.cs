using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Core.Persistence.DatabaseModelDefinitions
{
    /// <summary>
    /// Represents a database index definition retreived by querying the database
    /// </summary>
    internal class DbIndexDefinition
    {
        public virtual string IndexName { get; set; }
        public virtual string TableName { get; set; }
        public virtual string ColumnName { get; set; }
        public virtual bool IsUnique { get; set; }
    }

    internal static class DbIndexDefinitionExtensions
    {
        public static IEnumerable<DbIndexDefinition> GetDefinedIndexesDefinitions(this ISqlSyntaxProvider sql, Database db)
        {
            return sql.GetDefinedIndexes(db)
                .Select(x => new DbIndexDefinition()
                {
                    TableName = x.Item1,
                    IndexName = x.Item2,
                    ColumnName = x.Item3,
                    IsUnique = x.Item4
                }).ToArray();
        }
    }
}