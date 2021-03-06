﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcCMS.Models;
using System.Data.Entity;
namespace MvcCMS.Data
{
    public class PostRepository : IPostRepository
    {
        public int CountPublished
        {
            get
            {
                using (var db = new CmsContext())
                {
                    return db.Posts.Count(p => p.Published < DateTime.Now);
                }
            }
        }

        public async Task CreateAsync(Post model)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == model.Id);
                if(post != null)
                {
                    throw new ArgumentException("A post with the id of" + model.Id + "already exists");
                }
                db.Posts.Add(model);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string id)
        {
            using(var db = new CmsContext())
            {
                var post = db.Posts.FirstOrDefault(p => p.Id == id);
                if (post == null)
                {
                    throw new KeyNotFoundException("The post with the id" + id + " doesnot exist");
                }

                
                db.Posts.Remove(post);
                await db.SaveChangesAsync();
            }
        }

        public async Task EditAsync(string id, Post updatedItem)
        {
            using(var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == id);
                if (post == null)
                {
                    throw new KeyNotFoundException("A post with the id of" + id 
                        + "does not exist in the data store.");
                }
         
                post.Id = updatedItem.Id;
                post.Title = updatedItem.Title;
                post.Content = updatedItem.Content;
                post.Published = updatedItem.Published;
                post.Tags = updatedItem.Tags;

                await db.SaveChangesAsync();
            }
        }

        public Post Get(string id)
        {
           using(var db = new CmsContext())
            {
                return db.Posts.Include("Author")
                    .SingleOrDefault(post => post.Id == id);
            }
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            using(var db = new CmsContext())
            {
                return await db.Posts.Include("Author")
                    .OrderByDescending(post => post.Created)
                    .ToArrayAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetPageAsync(int pageNumber, int pageSize)
        {
            using(var db = new CmsContext())
            {
                var skip = (pageNumber - 1) * pageSize;

                return await db.Posts.Where(p => p.Published < DateTime.Now)
                    .Include("Author")
                    .OrderByDescending(p => p.Published)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToArrayAsync();
           
            }
        }

        public async Task<IEnumerable<Post>> GetPostsByAuthorAsync(string authorId)
        {
            using (var db = new CmsContext())
            {
                return await db.Posts.Include("Author").Where(post =>post.AuthorId == authorId)
                    .OrderByDescending(post => post.Created)
                    .ToArrayAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetPostsByTagIdAsync(string tagId)
        {
             using(var db = new CmsContext())
            {
                var posts = await db.Posts.Include("Author")
                    .Where(post => post.CombinedTags.Contains(tagId)).ToListAsync(); ;

               return posts.Where(post =>
               post.Tags.Contains(tagId, StringComparer.CurrentCultureIgnoreCase)).ToList();
            }
        }

        public async Task<IEnumerable<Post>> GetPublishedPostsAsync()
        {
            using(var db = new CmsContext())
            {
                return await db.Posts.Include("Author").Where(post => post.Published < DateTime.Now)
                    .OrderByDescending(p => p.Published)
                    .ToArrayAsync();
            }
        }
    }
}
