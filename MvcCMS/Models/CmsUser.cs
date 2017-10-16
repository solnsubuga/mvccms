using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCMS.Models
{
  public class CmsUser: IdentityUser
    {
        public string DisplayName { get; set; }
        
    }
}
