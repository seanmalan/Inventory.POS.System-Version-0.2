﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVApp.Models
{
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int CategoryID { get; set; } // Primary key

        [Unique]
        public string CategoryName { get; set; } // Category name, unique and required
    }
}
