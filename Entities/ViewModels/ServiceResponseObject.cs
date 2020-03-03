using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{ 
    public enum ResponseResult
    {
        OK,
        Error
    }
   public  class ServiceResponseObject<T>
        where T: BaseResponseObject,new()
    {
        public ResponseResult Status { get; set; }
        public string Message { get; set; }

        public T ResponseData { get; set; }
    }
}
