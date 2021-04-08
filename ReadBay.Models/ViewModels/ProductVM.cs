using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBay.Models.ViewModels
{
    public class ProductVM
    {
        // View Model Specific to the product view Holds Product Object, Category List & Book Type for drop down
        
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> BookTypeList { get; set; }

    }
}
