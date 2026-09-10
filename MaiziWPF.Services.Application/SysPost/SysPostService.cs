using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Application
{
    public class SysPostService : ISysPostService
    {
        private readonly ISysPostRepository _repository;

        public SysPostService(ISysPostRepository repository)
        {
            _repository = repository;
        }

        public List<SysPost> SelectPostList(QueryPostInput post)
        {
            return _repository.SelectPostList(post);
        }

        public SysPost SelectPostById(long postId)
        {
            return _repository.SelectPostById(postId);
        }

        public int InsertPost(SysPost post)
        {
            return _repository.InsertPost(post);
        }

        public int UpdatePost(SysPost post)
        {
            return _repository.UpdatePost(post);
        }

        public int DeletePostById(long postId)
        {
            return _repository.DeletePostById(postId);
        }

        public bool CheckPostNameUnique(SysPost post)
        {
            return _repository.CheckPostNameUnique(post);
        }

        public bool CheckPostCodeUnique(SysPost post)
        {
            return _repository.CheckPostCodeUnique(post);
        }

        public bool CheckPostExistUser(long postId)
        {
            return _repository.CheckPostExistUser(postId);
        }
    }
}