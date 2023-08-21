using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brighteye_Test
{

    public class MyDbContext : DbContext
    {
        public DbSet<Table1> Table1 { get; set; }
        public DbSet<Table2> Table2 { get; set; }

        public MyDbContext() : base("DefaultConnection")
        {
            Table1 = Set<Table1>();
            Table2 = Set<Table2>();

        }



    }

    public class Table1
    {
        [Key]
        public int Id { get; set; }
        public int Value { get; set; }
    }

    public class Table2
    {
        [Key]
        public int Id { get; set; }
        public int Value { get; set; }
    }
}
