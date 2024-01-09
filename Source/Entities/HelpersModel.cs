using VMS.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VMS.Entities
{
    public class HelpersModel
    {
        public partial class StringViewModel
        {
            public string Msg { get; set; }
        }

        public partial class SimpleViewModel
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string Token { get; set; }
        }

        public class UserData
        {
            public string UserId { get; set; }
            public string Token { get; set; }

        }
    }

    public partial class Logs : BaseEntity
    {
        public string Description { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Note1 { get; set; }
        public string ErrorLog { get; set; }
        public string StatusLog { get; set; }

    }

    public class LogsExt 
    {
        public string Id { get; set; }
        public string Result { get; set; }
    }

    public class LogsDto
    {
        //public bool Status { get; set; }
        public string FlagData { get; set; }
        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? Module { get; set; }
        public string? Action { get; set; }
        public string? Request { get; set; }
        public string? Response { get; set; }
        public string? Note { get; set; }
        public string? ErrorLog { get; set; }
        public string? StatusLog { get; set; }
        public string UserId { get; set; }
    }

    public class dataAuthExt
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class dataAuth
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }

    public class ListPage
    {
        public string Keyword { get; set; }
        public string fields { get; set; }
        public string PageSize { get; set; }
        public string PageNumber { get; set; }
        public string JSONFilter { get; set; }
    }

    public class ListPageExt 
    {
        public string Search { get; set; }
        public int? Page { get; set; }
        public int? Size { get; set; }
        public string OrderBy { get; set; }        
        public string UserId { get; set; }
        public string request { get; set; }
    }

    public class ListPageDetail
    {
        public string KeyHeader { get; set; }
        public string Keyword { get; set; }
        public string fields { get; set; }
        public string PageSize { get; set; }
        public string PageNumber { get; set; }
        public string JSONFilter { get; set; }
    }

    public class BaseRequestGridModel
    {
        public string WhereClause { get; set; }
        public string OrderBy { get; set; }
        public string Size { get; set; }
        public string Limit { get; set; }
        public string Offset { get; set; }
    }
    public class GridRequest
    {
        public string cmd { get; set; }
        public List<object> selected { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public List<Search> search { get; set; }
        public string searchLogic { get; set; }
        public List<Sort> sort { get; set; }
    }
    public class Search
    {
        public string field { get; set; }
        public string type { get; set; }
        public string @operator { get; set; }
        public object value { get; set; }
        public object svalue { get; set; }
    }
    public class Sort
    {
        public string field { get; set; }
        public string direction { get; set; }
    }

    public class ListParm
    {
        public string? Field { get; set; }
        public string? Keyword { get; set; }
    }

    public class ParmId
    { 
        public long Id { get; set; }
    }

    public class TokenExt
    {
        public string Token { get; set; }        
        public string CurrentUser { get; set; }
        public string ValidTo { get; set; }
        public string Message { get; set; }
    }

    public sealed class ErrorMessage
    {
        public ErrorMessage(string message, int errorCode, TokenExt Detail)
        {
            Message = message;
            ErrorCode = errorCode;
            this.Detail = Detail;
        }

        public string Message { get; }

        public int ErrorCode { get; }

        public TokenExt Detail { get; }
    }

    public class ListCombo
    {
        public string Code { get; set; }
        public string Search { get; set; }
        public int? Page { get; set; }
        public int? Size { get; set; }
        public string OrderBy { get; set; }
        public string UserId { get; set; }
        public string FilterData { get; set; }
        //public string request { get; set; }
    }

    public class RsList 
    {
        public int TotalRecords { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public object Data { get; set; }
    }
}
