using App.Utils.Serializable;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace App.Shared.Models
{
    public class AnswerPagedList<T>
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public List<T> Items { get; }

        public int IndexFrom { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage { get; }
        public bool HasNextPage { get; }

        public AnswerPagedList(bool isSuccess, string message, List<T> data, int indexFrom = 0, int pageIndex = 0, int pageSize = 0, int totalCount = 0, int totalPages = 0, bool hasPreviousPage = false, bool hasNextPage = false)
        {
            IsSuccess = isSuccess;
            Message = message;
            Items = data;

            IndexFrom = indexFrom;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = totalPages;
            HasPreviousPage = hasPreviousPage;
            HasNextPage = hasNextPage;
        }
    }

    /*
     0 - Тизим ҳатоси
     1 - ОК
     2 - Параметрларда ҳато (валидация)
     5 - Маълумот топилмади
     7 - Маълумот мавжуд
     */
    public class Answer<T>
    {
        public int Code { get; }
        public string Message { get; }
        public string Comment { get; }
        public T Data { get; }

        /// <summary>
        ///  Code:  
        ///  0 - Тизим ҳатоси
        ///  1 - ОК
        ///  2 - Параметрларда ҳато (валидация)
        ///  5 - Маълумот топилмади
        ///  7 - Маълумот мавжуд
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Message"></param>
        /// <param name="comment"></param>
        /// <param name="Data"></param>
        public Answer(int Code, string Message, string comment, T Data = default)
        {
            this.Code = Code;
            this.Message = Message;
            Comment = comment;
            this.Data = Data;
        }
    }

    public class AnswerPaging<T>
    {
        public AnswerPaging(int code, string message, int count, T data, int totalPages, int currentPage)
        {
            Code = code;
            Message = message;
            TotalCount = count;
            Data = data;
            TotalPages = totalPages;
            CurrentPage = currentPage;
        }

        public int Code { get; set; }
        public string Message { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public T Data { get; set; }
    }

    public class AnswerBasic
    {
        public int code { get; set; }
        public string message { get; set; }        
    }

    public class AnswerSignalResponse
    {
        public int Code { get; }
        public string Message { get; }
        public string Cmd { get; }
        public AnswerSignalResponse(int code, string message, string cmd)
        {
            Code = code;
            Message = message;
            Cmd = cmd;
        }

        public static string Create(int code, string message, string cmd)
        {
            var res = new AnswerSignalResponse(code, message, cmd);
            return res.ToJson();
        }
    }

}
