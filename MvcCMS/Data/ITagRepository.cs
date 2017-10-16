using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCMS.Data
{
   public interface ITagRepository
    {
        IEnumerable<string> GetAll();
        //bool Exists(string tag);
        Task EditAsync(string existingTag, string newTag);
        Task DeleteAsync(string tag);
        string Get(string tag);
    }
}
