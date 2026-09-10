using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Application
{
    public class SysDictService : ISysDictService
    {
        private readonly ISysDictRepository _repository;

        public SysDictService(ISysDictRepository repository)
        {
            _repository = repository;
        }

        public List<SysDictData> SelectDictDataByType(string dictType)
        {
            return _repository.SelectDictDataByType(dictType);
        }

        public List<SysDictType> SelectDictTypeList(QueryDictTypeInput input)
        {
            return _repository.SelectDictTypeList(input);
        }

        public SysDictType SelectDictTypeById(long dictId)
        {
            return _repository.SelectDictTypeById(dictId);
        }

        public int InsertDictType(SysDictType dictType)
        {
            return _repository.InsertDictType(dictType);
        }

        public int UpdateDictType(SysDictType dictType)
        {
            return _repository.UpdateDictType(dictType);
        }

        public int DeleteDictTypeById(long dictId)
        {
            return _repository.DeleteDictTypeById(dictId);
        }

        public bool CheckDictTypeUnique(SysDictType dictType)
        {
            return _repository.CheckDictTypeUnique(dictType);
        }

        public List<SysDictData> SelectDictDataList(QueryDictDataInput input)
        {
            return _repository.SelectDictDataList(input);
        }

        public SysDictData SelectDictDataById(long dictCode)
        {
            return _repository.SelectDictDataById(dictCode);
        }

        public int InsertDictData(SysDictData dictData)
        {
            return _repository.InsertDictData(dictData);
        }

        public int UpdateDictData(SysDictData dictData)
        {
            return _repository.UpdateDictData(dictData);
        }

        public int DeleteDictDataById(long dictCode)
        {
            return _repository.DeleteDictDataById(dictCode);
        }
    }
}