using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCMS.Data
{
    public class TagRepository : ITagRepository
    {
        public async Task DeleteAsync(string tag)
        {
            using (var db = new CmsContext())
            {
                var posts = db.Posts.Where(post => post.CombinedTags.Contains(tag)).ToList();

                posts = posts.Where(post =>
                post.Tags.Contains(tag, StringComparer.CurrentCultureIgnoreCase)).ToList();


                if (!posts.Any())
                {
                    throw new KeyNotFoundException("The tag " + tag + "does not exist");
                }

                foreach (var post in posts)
                {
                    post.Tags.Remove(tag);
  
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task EditAsync(string existingTag, string newTag)
        {
            using (var db = new CmsContext())
            {

                var posts = db.Posts.Where(post => post.CombinedTags.Contains(existingTag)).ToList();

                posts = posts.Where(post => 
                post.Tags.Contains(existingTag, StringComparer.CurrentCultureIgnoreCase)).ToList();

                if (!posts.Any())
                {
                    throw new KeyNotFoundException("The tag " + existingTag + "does not exist");
                }
                foreach(var post in posts)
                {
                    post.Tags.Remove(existingTag);
                    post.Tags.Add(newTag);
                }

                await db.SaveChangesAsync();
            }
        }

        public string Get(string tag)
        {
            using (var db = new CmsContext())
            {
                var posts = db.Posts.Where(post => post.CombinedTags.Contains(tag)).ToList(); 


                 posts = posts.Where(post =>
                post.Tags.Contains(tag, StringComparer.CurrentCultureIgnoreCase)).ToList();

                if (!posts.Any())
                {
                    throw new KeyNotFoundException("The tag " + tag + "does not exist");
                }
                return tag.ToLower();
            }
        }

        public IEnumerable<string> GetAll()
        {
            using (var db = new CmsContext())
            {
                var tagCollection = db.Posts.Select(p => p.CombinedTags).ToList();
                return string.Join(",", tagCollection).Split(',').Distinct();
            }
        }
    }
}
