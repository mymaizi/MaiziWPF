using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.MySql
{
    public class SysPostRepository : BaseRepository<SysPost, int>, ISysPostRepository
    {
        private readonly IFreeSql _fsql;

        public SysPostRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysPost> SelectPostList(QueryPostInput input)
        {
            System.Linq.Expressions.Expression<Func<SysPost, bool>> where = d => d.DelFlag == "0";

            if (!string.IsNullOrEmpty(input.PostCode))
                where = where.And(u => u.PostCode.Contains(input.PostCode));
            if (!string.IsNullOrEmpty(input.PostCategory))
                where = where.And(u => u.PostCategory.Contains(input.PostCategory));
            if (!string.IsNullOrEmpty(input.PostName))
                where = where.And(u => u.PostName.Contains(input.PostName));
            if (input.DeptId > 0)
                where = where.And(u => u.DeptId == input.DeptId);
            if (!string.IsNullOrEmpty(input.Status))
                where = where.And(u => u.Status == input.Status);

            return _fsql.Select<SysPost>()
                       .Where(where)
                       .Page(input)
                       .ToList();
        }

        public SysPost SelectPostById(long postId)
        {
            return _fsql.Select<SysPost>()
                .Where(p => p.PostId == postId && p.DelFlag == "0")
                .First();
        }

        public int InsertPost(SysPost post)
        {
            post.CreateTime = DateTime.Now;
            return (int)_fsql.Insert(post).ExecuteAffrows();
        }

        public int UpdatePost(SysPost post)
        {
            post.UpdateTime = DateTime.Now;
            return _fsql.Update<SysPost>().SetSource(post).ExecuteAffrows();
        }

        public int DeletePostById(long postId)
        {
            return _fsql.Update<SysPost>()
                .Set(p => p.DelFlag, "2")
                .Where(p => p.PostId == postId)
                .ExecuteAffrows();
        }

        public bool CheckPostNameUnique(SysPost post)
        {
            var query = _fsql.Select<SysPost>()
                .Where(p => p.PostName == post.PostName && p.DelFlag == "0");
            if (post.PostId != 0)
                query = query.Where(p => p.PostId != post.PostId);
            return !query.Any();
        }

        public bool CheckPostCodeUnique(SysPost post)
        {
            var query = _fsql.Select<SysPost>()
                .Where(p => p.PostCode == post.PostCode && p.DelFlag == "0");
            if (post.PostId != 0)
                query = query.Where(p => p.PostId != post.PostId);
            return !query.Any();
        }

        public bool CheckPostExistUser(long postId)
        {
            return _fsql.Select<SysUserPost>()
                .Where(p => p.PostId == postId)
                .Any();
        }
    }
}