using System;
using System.ComponentModel.DataAnnotations;

namespace ClubsCore.Paging
{
    public class QueryClubParameters
    {
        private const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        [Range(1, maxPageSize)]
        public int PageSize { get; set; }

        public string ClubName;
    }
}