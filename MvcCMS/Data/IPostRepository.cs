using MvcCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCMS.Data
{
    public interface IPostRepository
    {
       int CountPublished { get; }
       Post Get(string id);
       Task EditAsync(string id, Post item);
       Task CreateAsync(Post model);
       Task<IEnumerable<Post>> GetAllAsync();
        Task<IEnumerable<Post>> GetPostsByAuthorAsync(string authorId);
        Task DeleteAsync(string id);
        Task<IEnumerable<Post>> GetPublishedPostsAsync();
        Task<IEnumerable<Post>> GetPostsByTagIdAsync(string tagId);

        Task<IEnumerable<Post>> GetPageAsync(int pageNumber, int pageSize);
    }
}
