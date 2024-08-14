using Idams.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Infrastructure.EntityFramework.Core
{
    public class FilterModel
    {
        public string Column { get; }
        public string Operator { get; }
        public string Value { get; }

        public FilterModel(string columnName, string @operator, string value)
        {
            Column = columnName;
            Operator = @operator;
            Value = value;
        }

        public static string BuildFilter(List<List<FilterModel>> param)
        {
            StringBuilder sb = new();
            for (int i = 0; i < param.Count; i++)
            {
                List<FilterModel>? paramItem = param[i];
                sb.Append(i == 0 ? " where (" : " AND ");
                for (int j = 0; j < paramItem.Count; j++)
                {
                    FilterModel filter = paramItem[j];
                    sb.Append(j == 0 ? "(" : " OR ");

                    if (filter.Operator == FilterBuilderEnum.LIKE)
                    {
                        sb.Append("LOWER(" + filter.Column + ") like LOWER('%" + filter.Value + "%')");
                    }
                    else if(filter.Value == FilterBuilderEnum.NULL)
                    {
                        sb.Append($"{filter.Column} {filter.Operator} {filter.Value}");
                    }
                    else
                    {
                        sb.Append($"{filter.Column} {filter.Operator} '{filter.Value}'");
                    }

                    if(j == paramItem.Count - 1)
                    {
                        sb.Append(')');
                    }
                }
                if(i == param.Count - 1)
                {
                    sb.Append(')');
                }
            }
            return sb.ToString();
        }
    }
}
