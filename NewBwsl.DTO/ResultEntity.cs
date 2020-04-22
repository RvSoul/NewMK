using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace NewMK.DTO
{
    public class ResultEntity<T>
    {
        private bool issucess = false;
        private int count = 0;
        /// <summary>
        /// 是否成功--默认为false
        /// </summary>
        public bool IsSuccess { get { return issucess; } set { issucess = value; } }
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 返回的数据
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        ///  返回的结果总数
        /// </summary>
        public int Count { get { return count; } set { count = value; } }
    }


    public class ResultEntityUtil<T>
    {
        /// <summary>
        /// 返回成功的响应实体
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResultEntity<T> Success(T data, int count, string message = "操作成功")
        {
            return new ResultEntity<T>
            {
                Data = data,
                IsSuccess = true,
                Count = count,
                Msg = message
            };
        }

        /// <summary>
        /// 返回成功的响应实体
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResultEntity<T> Success(T data, string message = "操作成功")
        {
            return Success(data, 1, message);
        }

        /// <summary>
        /// 返回失败的响应实体
        /// </summary>
        /// <param name="data"></param>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResultEntity<T> Failure(T data, int errorCode, string message = "操作失败")
        {
            return new ResultEntity<T>
            {
                Data = data,
                IsSuccess = false,
                ErrorCode = errorCode,
                Msg = message
            };
        }

        /// <summary>
        /// 返回失败的响应实体
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResultEntity<T> Failure(T data, string message = "操作失败")
        {
            return Failure(data, Convert.ToInt32(Utility.ApiResultCode.Error), message);
        }

        /// <summary>
        /// 返回失败的响应实体
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResultEntity<bool> Failure(string message = "操作失败")
        {
            return new ResultEntity<bool>
            {
                IsSuccess = false,
                ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error),
                Msg = message
            };
        }
    }
}
