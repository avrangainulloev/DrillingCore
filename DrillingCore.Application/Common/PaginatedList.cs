using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Common
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }

        public PaginatedList() { }

        public PaginatedList(List<T> items, int totalCount, int page, int limit)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            Limit = limit;
        }
    }
}
