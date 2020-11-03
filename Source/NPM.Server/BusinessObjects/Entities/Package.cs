using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPM.BusinessObjects.Entities
{
    [Table("Packages")]
    public class Package
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        [StringLength(25)]
        public string Version { get; set; }
    }
}
