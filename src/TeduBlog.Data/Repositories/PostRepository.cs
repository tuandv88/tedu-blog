﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TeduBlog.Core.Domain.Content;
using TeduBlog.Core.Models;
using TeduBlog.Core.Models.Content;
using TeduBlog.Core.Repositories;
using TeduBlog.Data.SeedWorks;

namespace TeduBlog.Data.Repositories
{
	public class PostRepository : RepositoryBase<Post, Guid>, IPostRepository
	{
		private readonly IMapper _mapper;
		public PostRepository(TeduBlogContext context, IMapper mapper) : base(context)
		{
			_mapper = mapper;
		}

		public Task<List<Post>> GetPopularPosts(int count)
		{
			return _context.Posts.OrderByDescending(x => x.ViewCount).Take(count).ToListAsync();
		}

		public async Task<PagedResult<PostInListDTO>> GetPostsPagingAsync(string? keyword, Guid? categoryId, int pageIndex = 1, int pageSize = 10)
		{
			var query = _context.Posts.AsQueryable();
			if(!string.IsNullOrEmpty(keyword) )
			{
				query = query.Where(x => x.Name.Contains(keyword));
			}

			if(categoryId.HasValue)
			{
				query = query.Where(x => x.CategoryId == categoryId.Value);
			}

			var totalRow = await query.CountAsync();

			query.OrderByDescending(x => x.DataCreated)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize);

			return new PagedResult<PostInListDTO>
			{
				Results = await _mapper.ProjectTo<PostInListDTO>(query).ToListAsync(),
				CurrentPage = pageIndex,
				RowCount = totalRow,
				PageSize = pageSize
			};
		}
	}
}
