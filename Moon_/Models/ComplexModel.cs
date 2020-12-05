using Moon.Entities;
using Moon.Models;
using PagedList.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moon_.Models
{
    public class ComplexModel
    {
        public IPagedList<Files> _PaginatedList { get; set; }
        public IEnumerable<Files> _FavoriteList { get; set; }

        public ComplexModel(IPagedList<Files> PaginatedList, IEnumerable<Files> FavoriteList) {
            _PaginatedList = PaginatedList;
            _FavoriteList = FavoriteList;
        }
        public ComplexModel() { }
    }
}
